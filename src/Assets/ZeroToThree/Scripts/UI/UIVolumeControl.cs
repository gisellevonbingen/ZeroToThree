using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIVolumeControl : UIObject
    {
        public const int Factor = 100;

        [Header("Editor")]
        public UIImage MinusButton;
        public UIImage PlusButton;
        public UISlider Slider;
        public UILabel NameLabel;
        public UILabel ValueLabel;
        public float ClickAmount;
        public float PushModeEnterDuration;
        public float PushModeAmount;
        public float PushModeAccel;

        [Header("Status")]
        public float PushDuration;
        public UIImage PushButton;
        public bool PushMode;
        public float PushStartValue;
        public float PushedValue;

        public event EventHandler ValueChanged;

        protected override void Awake()
        {
            base.Awake();

            this.MinusButton.TouchButtonDown += this.OnButtonTouchDown;
            this.MinusButton.TouchButtonUp += this.OnButtonTouchUp;

            this.PlusButton.TouchButtonDown += this.OnButtonTouchDown;
            this.PlusButton.TouchButtonUp += this.OnButtonTouchUp;

            this.Slider.ValueChanged += this.OnSliderValueChanged;

            this.ResetPushState(null);
        }

        public float Value
        {
            get
            {
                return this.Slider.Value / Factor;
            }

            set
            {
                this.Slider.Value = value * Factor;
            }

        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            this.ValueChanged?.Invoke(this, e);
        }

        protected override void Start()
        {
            base.Start();

            this.Slider.MinValue = 0.0F * Factor;
            this.Slider.MaxValue = 1.0F * Factor;

            this.UpdateText();
        }

        protected override void Update()
        {
            base.Update();

            var pushButton = this.PushButton;

            if (pushButton != null)
            {
                var delta = Time.deltaTime;
                var pushDuration = this.PushDuration;
                var pushModeEnterDuration = this.PushModeEnterDuration;

                if (pushDuration >= pushModeEnterDuration)
                {
                    this.PushMode = true;
                    var pushedValue = this.PushedValue;

                    this.Slider.Value = this.PushStartValue + this.GetDirectedAmount(this.PushButton, pushedValue);
                    this.PushedValue = ((pushedValue + this.PushModeAmount * delta) + (pushedValue * this.PushModeAccel));
                }

                this.PushDuration += delta;
            }

        }

        private void OnButtonTouchDown(object sender, UITouchButtonEventArgs e)
        {
            this.ResetPushState(sender as UIImage);
        }

        private void ResetPushState(UIImage button)
        {
            this.PushDuration = 0.0F;
            this.PushButton = button;
            this.PushMode = false;
            this.PushStartValue = this.Slider.Value;
            this.PushedValue = 0.0F;
        }

        private void OnButtonTouchUp(object sender, UITouchButtonEventArgs e)
        {
            if (this.PushMode == false)
            {
                this.Slider.Value += this.GetDirectedAmount(this.PushButton, this.ClickAmount);
            }

            this.ResetPushState(null);
        }

        private float GetDirectedAmount(UIImage image, float amount)
        {
            if (image == this.MinusButton)
            {
                return -amount;
            }
            else if (image == this.PlusButton)
            {
                return +amount;
            }

            return 0.0F;
        }

        private void OnSliderValueChanged(object sender, EventArgs e)
        {
            var slider = this.Slider;
            var value = slider.Value;
            var floor = Mathf.Floor(slider.Value);

            if (value != floor)
            {
                slider.Value = floor;
            }

            this.UpdateText();

            this.OnValueChanged(e);
        }

        private void UpdateText()
        {
            this.ValueLabel.Text.text = this.Slider.Value.ToString("0") + "%";
        }

    }

}
