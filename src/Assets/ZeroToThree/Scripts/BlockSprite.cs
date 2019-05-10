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
    public class BlockSprite : UIObject
    {
        public UIImage TileRenderer;
        public UIImage BreakRenderer;
        public UIImage TileMaskRenderer;
        public UILabel Text;
        public Color[] ValueColors;

        public Block Block { get; set; }

        public float ZoomOutMin;
        public float ZoomOutDuration;
        public float ZoomInDuration;
        public float BreakDuration;
        public float Gravity;

        public Vector2 GoalPosition;
        public bool InPosition;
        public bool IsBreaked;
        public bool IsMasked;

        public event EventHandler Breaked;
        public event EventHandler Masked;

        public Coroutine AnimatingRoutine;

        protected override UIObject QueryChildren(Vector2 worldPosition)
        {
            return null;
        }

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

        public void Reset()
        {
            this.Block = null;
            this.StopAllCoroutines();

            this.GoalPosition = new Vector2();
            this.InPosition = false;
            this.IsBreaked = false;
            this.IsMasked = false;

            this.Text.gameObject.SetActive(true);
            this.TileRenderer.gameObject.SetActive(true);
            this.BreakRenderer.gameObject.SetActive(false);
            this.TileMaskRenderer.gameObject.SetActive(false);

            this.transform.localScale = new Vector3(1.0F, 1.0F, 1.0F);
        }

        private void Update()
        {

        }

        public void UpdateValue()
        {
            var block = this.Block;

            if (block != null)
            {
                var value = block.Value;
                this.TileRenderer.Image.color = this.ValueColors[value];
                this.Text.Text.text = value.ToString();
            }
        }

        public Vector2 GetTileSize()
        {
            return this.transform.sizeDelta;
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
            tileRenderer.gameObject.SetActive(false);

            var text = this.Text;
            text.gameObject.SetActive(false);

            var breakRenderer = this.BreakRenderer;
            breakRenderer.Image.color = tileRenderer.Image.color;
            breakRenderer.gameObject.SetActive(true);

            var tileMaskRenderer = this.TileMaskRenderer;
            tileMaskRenderer.gameObject.SetActive(false);

            while (true)
            {
                var time = Time.time;

                if (time - startStamp >= this.BreakDuration)
                {
                    this.IsBreaked = true;

                    tileRenderer.gameObject.SetActive(true);
                    text.gameObject.SetActive(true);
                    breakRenderer.gameObject.SetActive(false);

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
            var startStamp = 0.0F;
            var min = this.ZoomOutMin;
            var max = 1.0F;

            int phase = 0;

            while (true)
            {
                var time = Time.time;

                if (phase == 0)
                {
                    startStamp = Time.time;
                    phase = 1;
                }
                else if (phase == 1)
                {
                    var zoomOutDuration = this.ZoomOutDuration;
                    var ratio = Math.Min((time - startStamp) / zoomOutDuration, 1.0F);
                    var scale = max - (max - min) *  ratio;
                    this.transform.localScale = new Vector3(scale, scale, scale);

                    if (time - startStamp >= zoomOutDuration)
                    {
                        startStamp = Time.time;
                        phase = 2;
                    }

                }
                else if (phase == 2)
                {
                    this.UpdateValue();

                    var tileMaskRenderer = this.TileMaskRenderer;
                    tileMaskRenderer.gameObject.SetActive(true);

                    var zoomInDuration = this.ZoomInDuration;
                    var ratio = Math.Min((time - startStamp) / zoomInDuration, 1.0F);
                    var scale = max - (max - min) * (1.0F - ratio);
                    this.transform.localScale = new Vector3(scale, scale, scale);

                    if (time - startStamp >= zoomInDuration)
                    {
                        startStamp = Time.time;
                        phase = 3;
                    }

                }
                else if (phase == 3)
                {
                    this.IsMasked = true;
                    this.OnMasked();

                    break;
                }

                yield return null;
            }

        }

        private void OnMasked()
        {
            this.Masked?.Invoke(this, new EventArgs());
        }

        public bool CanMask()
        {
            if (this.AnimatingRoutine != null || this.IsMasked == true)
            {
                return false;
            }

            return true;
        }

    }

}
