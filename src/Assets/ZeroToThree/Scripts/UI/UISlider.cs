using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UISlider : UIObject
    {
        [Header("Editor")]
        public UIImage BackLight;
        public UIImage BackDark;
        public UIImage Handle;

        private float _Value;
        public float Value { get => this._Value; set { this._Value = Mathf.Clamp01(value); this.OnValueChagned(EventArgs.Empty); } }
        public event EventHandler ValueChanged;

        private bool Handling;

        protected override void Awake()
        {
            base.Awake();

            this.Value = 0.0F;

            this.Handling = false;

            this.Handle.TouchButtonDown += this.OnTouchButtonDown;
            this.Handle.TouchButtonUp += this.OnTouchButtonUp;
            this.BackLight.TouchButtonDown += this.OnTouchButtonDown;
            this.BackLight.TouchButtonUp += this.OnTouchButtonUp;
            this.BackDark.TouchButtonDown += this.OnTouchButtonDown;
            this.BackDark.TouchButtonUp += this.OnTouchButtonUp;
        }

        protected virtual void OnValueChagned(EventArgs e)
        {
            var backLight = this.BackLight;
            var backDark = this.BackDark;
            var handle = this.Handle;
            var width = backLight.transform.rect.width;
            var value = this.Value;

            var handlePosition = handle.transform.localPosition;
            handlePosition.x = this.ValueToPoint(value);
            handle.transform.localPosition = handlePosition;

            backLight.Image.fillAmount = value;
            backDark.Image.fillAmount = 1.0F - value;

            this.ValueChanged?.Invoke(this, e);
        }

        protected override void Update()
        {
            base.Update();

            if (this.Handling == true)
            {
                this.UpdateValueByHandle();
            }

        }

        private void UpdateValueByHandle()
        {
            var mousePosition = GameManager.Instance.UIManager.MousePosition;
            var value = this.PointToValue(mousePosition.x);

            this.Value = value;
        }

        private float ValueToPoint(float value)
        {
            var handle = this.Handle;
            var backLight = this.BackLight;
            var backLightRect = backLight.transform.rect;
            var padding = (handle.transform.rect.width - backLightRect.height) / 2.0F;
            var barWidth = backLightRect.width - padding - padding;
            var center = this.transform.position;

            var minPosition = new Vector3(center.x - barWidth / 2.0F, center.y, center.z);
            var point = minPosition.x + barWidth * value;

            return point;
        }

        private float PointToValue(float point)
        {
            var handle = this.Handle;
            var backLight = this.BackLight;
            var backLightRect = backLight.transform.rect;
            var padding = (handle.transform.rect.width - backLightRect.height) / 2.0F;
            var barWidth = backLightRect.width - padding - padding;
            var center = this.transform.position;

            var minPosition = new Vector3(center.x - barWidth / 2.0F, center.y, center.z);
            var value = Mathf.Clamp01((point - minPosition.x) / barWidth);

            return value;
        }

        private void OnTouchButtonDown(object sender, UITouchButtonEventArgs e)
        {
            this.Handling = true;
            this.UpdateValueByHandle();
        }

        private void OnTouchButtonUp(object sender, UITouchButtonEventArgs e)
        {
            this.Handling = false;
        }

    }

}
