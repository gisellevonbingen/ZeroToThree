﻿using System;
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

        public Block Block { get; set; }

        public float ZoomMinScale;
        public float ZoomMaxScale;
        public float ZoomOutDuration;
        public float ZoomInDuration;
        public float BreakDuration;
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
            this.Actions.Add(new UIActionMoveToSpeed() { End = newPosition, MaxDistanceDelta = this.Gravity });
        }

        public void Reset()
        {
            this.Block = null;

            this.GoalPosition = new Vector2();
            this.IsBreaked = false;
            this.IsMasked = false;

            this.Text.gameObject.SetActive(true);
            this.TileRenderer.gameObject.SetActive(true);
            this.BreakRenderer.gameObject.SetActive(false);
            this.TileMaskRenderer.gameObject.SetActive(false);

            var scale = this.ZoomMaxScale;
            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        protected override void Update()
        {
            base.Update();
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

            this.Actions.Add(new UIActionTimeDelegate()
            {
                Duration = this.BreakDuration,
                CompleteHandler = target =>
                {
                    this.IsBreaked = true;

                    tileRenderer.gameObject.SetActive(true);
                    text.gameObject.SetActive(true);
                    breakRenderer.gameObject.SetActive(false);

                    this.Breaked?.Invoke(this, new EventArgs());
                }

            });

        }

        public void MaskStart()
        {
            var min = this.ZoomMinScale;
            var max = this.ZoomMaxScale;
            this.Actions.Add(new UIActionScaleToTime() { Duration = this.ZoomOutDuration, End = new Vector3(min, min, min) });
            this.Actions.Add(new UIActionDelegate()
            {
                CompleteHandler = (target) =>
                {
                    this.UpdateValue();

                    var tileMaskRenderer = this.TileMaskRenderer;
                    tileMaskRenderer.gameObject.SetActive(true);
                }

            });
            this.Actions.Add(new UIActionScaleToTime() { Duration = this.ZoomInDuration, End = new Vector3(max, max, max) });
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
