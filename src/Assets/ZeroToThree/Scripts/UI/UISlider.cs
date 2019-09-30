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

        private float _MinValue;
        public float MinValue { get => this._MinValue; set { this._MinValue = value; this.OnMinValueChagned(EventArgs.Empty); } }
        public event EventHandler MinValueChanged;

        private float _MaxValue;
        public float MaxValue { get => this._MaxValue; set { this._MaxValue = value; this.OnMaxValueChagned(EventArgs.Empty); } }
        public event EventHandler MaxValueChanged;

        private float _Value;
        public float Value { get => this._Value; set { this._Value = Mathf.Clamp(value, this.MinValue, this.MaxValue); this.OnValueChagned(EventArgs.Empty); } }
        public event EventHandler ValueChanged;

        private bool Handling;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.MinValue = 0.0F;
            this.MaxValue = 1.0F;
            this.Value = 0.0F;

            this.Handle.TouchButtonDown += this.OnTouchButtonDown;
            this.Handle.TouchButtonUp += this.OnTouchButtonUp;
            this.BackLight.TouchButtonDown += this.OnTouchButtonDown;
            this.BackLight.TouchButtonUp += this.OnTouchButtonUp;
            this.BackDark.TouchButtonDown += this.OnTouchButtonDown;
            this.BackDark.TouchButtonUp += this.OnTouchButtonUp;

            this.Handling = false;
        }

        protected virtual void OnMinValueChagned(EventArgs e)
        {
            this.Value = this.Value;

            this.MinValueChanged?.Invoke(this, e);
        }

        protected virtual void OnMaxValueChagned(EventArgs e)
        {
            this.Value = this.Value;

            this.MaxValueChanged?.Invoke(this, e);
        }

        protected virtual void OnValueChagned(EventArgs e)
        {
            var backLight = this.BackLight;
            var backDark = this.BackDark;
            var handle = this.Handle;
            var width = backLight.transform.rect.width;
            var value = this.Value;
            var percent = this.ValueToPercent(value);
            var point = this.PercentToPoint(percent);

            var handlePosition = handle.transform.localPosition;
            handlePosition.x = point.IsGeneral() ? point : 0.0F;
            handle.transform.localPosition = handlePosition;

            backLight.Image.fillAmount = percent;
            backDark.Image.fillAmount = 1.0F - percent;

            this.ValueChanged?.Invoke(this, e);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.Handling == true)
            {
                this.UpdateValueByHandle();
            }

        }

        private void UpdateValueByHandle()
        {
            var mousePosition = GameManager.Instance.UIManager.MousePosition;
            var percent = this.PointToPercent(mousePosition.x);
            var value = this.PercentToValue(percent);

            this.Value = value;
        }

        public float ValueToPercent(float value)
        {
            var minValue = this.MinValue;
            var maxValue = this.MaxValue;
            var percent = Mathf.Clamp01((value - minValue) / (maxValue - minValue));

            return percent;
        }

        public float PercentToValue(float percent)
        {
            var minValue = this.MinValue;
            var maxValue = this.MaxValue;
            var value = minValue + (maxValue - minValue) * percent;

            return value;
        }

        public float PercentToPoint(float percent)
        {
            var handle = this.Handle;
            var backLight = this.BackLight;
            var backLightRect = backLight.transform.rect;
            var padding = (handle.transform.rect.width - backLightRect.height) / 2.0F;
            var barWidth = backLightRect.width - padding - padding;
            var center = this.transform.position;

            var minPosition = new Vector3(center.x - barWidth / 2.0F, center.y, center.z);
            var point = minPosition.x + barWidth * percent;

            return point;
        }

        public float PointToPercent(float point)
        {
            var handle = this.Handle;
            var backLight = this.BackLight;
            var backLightRect = backLight.transform.rect;
            var padding = (handle.transform.rect.width - backLightRect.height) / 2.0F;
            var barWidth = backLightRect.width - padding - padding;
            var center = this.transform.position;

            var minPosition = new Vector3(center.x - barWidth / 2.0F, center.y, center.z);
            var percent = Mathf.Clamp01((point - minPosition.x) / barWidth);

            return percent;
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
