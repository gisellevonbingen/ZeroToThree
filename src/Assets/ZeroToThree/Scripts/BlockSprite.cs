using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ZeroToThree.Scripts;
using Assets.ZeroToThree.Scripts.UI;
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
        public AudioClip BreakAudio;
        public AudioClip MaskAudio;

        public Block Block { get; set; }

        public float ZoomMinDuration;
        public float ZoomMinScale;
        public float ZoomMaxDuration;
        public float ZoomMaxScale;
        public float BreakDuration;
        public float BreakRotation;
        public float Gravity;

        public Vector2 GoalPosition;
        public bool IsBreaked;
        public bool IsMasked;

        public event EventHandler Breaked;
        public event EventHandler Masked;

        protected override UIObject QueryChildren(Vector2 worldPosition)
        {
            return null;
        }

        public void MoveStart(Vector2 newPosition)
        {
            this.Actions.Add(new UIActionMoveToSpeed() { End = newPosition, Velocity = this.Gravity, Gain = 0.5F, InPosition = 90 });
        }

        public void Reset()
        {
            this.Block = null;

            this.Actions.Clear();

            this.GoalPosition = new Vector2();
            this.IsBreaked = false;
            this.IsMasked = false;

            this.Text.gameObject.SetActive(true);
            this.TileRenderer.gameObject.SetActive(true);
            this.BreakRenderer.gameObject.SetActive(false);
            this.TileMaskRenderer.gameObject.SetActive(false);

            var scale = this.ZoomMaxScale;
            this.transform.localScale = new Vector3(scale, scale, scale);
            this.transform.localRotation = new Quaternion(0.0F, 0.0F, 0.0F, 0.0F);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
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
            this.IsBreaked = false;
            this.IsMasked = false;

            var action = new UIActionSet();
            action.Actions.Add(new UIActionScaleToTime() { Duration = this.BreakDuration, End = new Vector3(0.0F, 0.0F, 0.0F) });
            action.Actions.Add(new UIActionRotateToTime() { Duration = this.BreakDuration, End = this.BreakRotation });
            this.Actions.Add(action);

            this.Actions.Add(new UIActionDelegate()
            {
                CompleteHandler = target =>
                {
                    this.IsBreaked = true;
                    this.Breaked?.Invoke(this, new EventArgs());
                }

            });
        }

        public void MaskStart()
        {
            var min = this.ZoomMinScale;
            var max = this.ZoomMaxScale;
            this.Actions.Add(new UIActionScaleToTime() { Duration = this.ZoomMinDuration, End = new Vector3(min, min, min) });
            this.Actions.Add(new UIActionDelegate()
            {
                CompleteHandler = (target) =>
                {
                    this.UpdateValue();

                    var tileMaskRenderer = this.TileMaskRenderer;
                    tileMaskRenderer.gameObject.SetActive(true);
                }

            });
            this.Actions.Add(new UIActionScaleToTime() { Duration = this.ZoomMaxDuration, End = new Vector3(max, max, max) });
            this.Actions.Add(new UIActionDelegate()
            {
                CompleteHandler = (target) =>
                {
                    this.IsMasked = true;
                    this.OnMasked();
                }

            });

        }

        private void OnMasked()
        {
            this.Masked?.Invoke(this, new EventArgs());
        }

        public bool CanMask()
        {
            if (this.IsMasked == true)
            {
                return false;
            }

            return true;
        }

    }

}
