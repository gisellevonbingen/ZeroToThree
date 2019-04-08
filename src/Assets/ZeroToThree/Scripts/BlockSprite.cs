using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ZeroToThree.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts
{
    public class BlockSprite : PoolingObject, IPointerClickHandler
    {
        public SpriteRenderer TileRenderer;
        public SpriteRenderer BreakRenderer;
        public SpriteRenderer TileMaskRenderer;
        public BoxCollider2D TileCollider;
        public Text Text;
        public Color[] ValueColors;

        public Block Block { get; private set; }

        public float MaskingDuration;
        public float Gravity;

        public Vector2 GoalPosition;
        public bool InPosition;
        public bool IsBreaked;
        public bool IsMasked;

        public event EventHandler Click;
        public event EventHandler Breaked;
        public event EventHandler Masked;

        public Coroutine AnimatingRoutine;

        public Coroutine NewAnimationRoutine(IEnumerator routine)
        {
            var prev = this.AnimatingRoutine;

            if (prev != null)
            {
                this.StopCoroutine(prev);
            }

            var next = this.StartCoroutine(this.Control(routine));
            this.AnimatingRoutine = next;

            return next;
        }

        private IEnumerator Control(IEnumerator routine)
        {
            yield return routine;
            this.AnimatingRoutine = null;
        }

        public void MoveStart(Vector2 newPosition)
        {
            this.NewAnimationRoutine(this.MoveRoutine(newPosition));
        }

        private IEnumerator MoveRoutine(Vector2 goalPosition)
        {
            this.GoalPosition = goalPosition;
            this.InPosition = false;

            while (true)
            {
                var position = this.transform.localPosition;
                var nextPosition = Vector2.MoveTowards(position, goalPosition, this.Gravity * Time.deltaTime);

                this.transform.localPosition = nextPosition;

                if (goalPosition.sqrMagnitude == nextPosition.sqrMagnitude)
                {
                    this.InPosition = true;
                    break;
                }
                else
                {
                    yield return null;
                }

            }

        }

        public override void OnObtain()
        {
            base.OnObtain();
        }

        public override void OnFree()
        {
            base.OnFree();
        }

        private void Start()
        {

        }

        public void Reset(Block block)
        {
            this.Block = block;

            this.StopAllCoroutines();

            this.GoalPosition = new Vector2();
            this.InPosition = false;
            this.IsBreaked = false;
            this.IsMasked = false;

            this.Text.enabled = true;
            this.TileRenderer.enabled = true;
            this.BreakRenderer.enabled = false;
            this.TileMaskRenderer.enabled = false;
        }

        private void Update()
        {
            var block = this.Block;

            if (block != null)
            {
                var value = block.Value;
                this.TileRenderer.color = this.ValueColors[value];
                this.Text.text = value.ToString();
            }

        }

        public Vector2 GetTileSize()
        {
            return this.TileCollider.size;
        }

        public void BreakStart()
        {
            this.NewAnimationRoutine(this.BreakRoutine());
        }

        private IEnumerator BreakRoutine()
        {
            this.IsBreaked = false;
            this.IsMasked = false;
            var startStamp = Time.time;

            var tileRenderer = this.TileRenderer;
            tileRenderer.enabled = false;

            var text = this.Text;
            text.enabled = false;

            var breakRenderer = this.BreakRenderer;
            breakRenderer.color = tileRenderer.color;
            breakRenderer.enabled = true;

            var tileMaskRenderer = this.TileMaskRenderer;
            tileMaskRenderer.enabled = false;

            while (true)
            {
                var time = Time.time;

                if (time - startStamp >= 0.5F)
                {
                    this.IsBreaked = true;

                    tileRenderer.enabled = true;
                    text.enabled = true;
                    breakRenderer.enabled = false;

                    this.Breaked?.Invoke(this, new EventArgs());
                    break;
                }
                else
                {
                    yield return null;
                }

            }

        }

        public void MaskStart()
        {
            this.NewAnimationRoutine(this.MaskRoutine());
        }

        public IEnumerator MaskRoutine()
        {
            this.IsMasked = false;
            var startStamp = Time.time;

            var tileMaskRenderer = this.TileMaskRenderer;
            tileMaskRenderer.enabled = true;

            while (true)
            {
                var time = Time.time;
                float maskingDuration = this.MaskingDuration;
                var alphaRatio = Math.Min((time - startStamp) / maskingDuration, 1.0F);

                var color = tileMaskRenderer.color;
                color.a = alphaRatio;
                tileMaskRenderer.color = color;

                if (time - startStamp >= maskingDuration)
                {
                    this.IsMasked = true;
                    this.Masked?.Invoke(this, new EventArgs());

                    break;
                }
                else
                {
                    yield return null;
                }


            }

        }

        public bool CanClick()
        {
            if (this.AnimatingRoutine != null || this.IsMasked == true)
            {
                return false;
            }

            return true;
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (this.CanClick() == true)
            {
                this.Click?.Invoke(this, new EventArgs());
            }

        }

    }

}
