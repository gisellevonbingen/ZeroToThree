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
        public UIObject ConnectionContainer;
        public UIImage TileMaskRenderer;
        public UILabel Text;
        public Color[] ValueColors;
        public AudioClip BreakAudio;
        public AudioClip MaskAudio;
        public BlockConnect BlockConnectPrefab;

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

        private Dictionary<BlockDirection, BlockConnect> Connections;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.Connections = new Dictionary<BlockDirection, BlockConnect>();
            var prefab = this.BlockConnectPrefab;
            var transform = this.ConnectionContainer.transform;

            foreach (var direction in BlockDirection.Values)
            {
                var connection = GameObject.Instantiate(prefab);
                connection.gameObject.SetActive(false);
                connection.transform.SetParent(transform);
                connection.name = "C:" + direction.Name;

                this.Connections[direction] = connection;
            }

            this.ClearConnection();
        }

        public void ClearConnection()
        {
            foreach (var pair in this.Connections)
            {
                var connection = pair.Value;
                connection.gameObject.SetActive(false);
            }

        }

        public void ClearConnection(BlockDirection direction)
        {
            var connection = this.Connections[direction];
            connection.gameObject.SetActive(false);
        }

        public void CreateConnection(BlockDirection direction)
        {
            var tileSize = this.GetTileSize();
            var connection = this.Connections[direction];
            var active = connection.gameObject.activeSelf;

            if (active == false)
            {
                connection.gameObject.SetActive(true);
                connection.Image.color = this.TileRenderer.Image.color;

                var width = Mathf.Lerp(connection.Short, connection.Long, Math.Abs(direction.Y));
                var height = Mathf.Lerp(connection.Short, connection.Long, Math.Abs(direction.X));
                var size = new Vector2(width, height);
                connection.transform.sizeDelta = size;
                connection.transform.localPosition = new Vector2(0.0F, 0.0F);

                var offsetX = +Mathf.LerpUnclamped(0.0F, tileSize.x / 2.0F, direction.X);
                var offsetY = -Mathf.LerpUnclamped(0.0F, tileSize.y / 2.0F, direction.Y);
                var offset = new Vector2(offsetX, offsetY);

                connection.Actions.Add(new UIActionMoveToSpeed() { End = offset, Velocity = 1920.0F * 0.5F });
            }

        }

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

            this.GoalPosition = new Vector2();
            this.IsBreaked = false;
            this.IsMasked = false;

            this.Text.gameObject.SetActive(true);
            this.TileRenderer.gameObject.SetActive(true);
            this.TileMaskRenderer.gameObject.SetActive(false);

            var scale = this.ZoomMaxScale;
            this.transform.localScale = new Vector3(scale, scale, scale);
            this.transform.localRotation = new Quaternion(0.0F, 0.0F, 0.0F, 0.0F);

            this.ClearConnection();
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

            this.ClearConnection();
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

            this.ClearConnection();
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
