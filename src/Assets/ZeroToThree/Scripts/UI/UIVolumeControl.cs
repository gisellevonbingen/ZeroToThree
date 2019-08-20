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
        [Header("Editor")]
        public UIImage Minus;
        public UIImage Plus;
        public UISlider Slider;
        public UILabel Text;
        public float ClickAmount;
        public float PushModeEnterDuration;

        [Header("Status")]
        public float PushDuration;
        public UIImage PushButton;
        public bool PushMode;

        protected override void Awake()
        {
            base.Awake();

            this.Minus.TouchButtonDown += this.OnButtonTouchDown;
            this.Minus.TouchButtonUp += this.OnButtonTouchUp;

            this.Plus.TouchButtonDown += this.OnButtonTouchDown;
            this.Plus.TouchButtonUp += this.OnButtonTouchUp;

            this.Slider.ValueChanged += this.OnSliderValueChanged;

            this.PushDuration = 0.0F;
            this.PushButton = null;
            this.PushMode = false;

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

                    this.UpdateValue(pushButton, delta * this.ClickAmount);
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
        }

        private void OnButtonTouchUp(object sender, UITouchButtonEventArgs e)
        {
            if (this.PushMode == false)
            {
                this.UpdateValue(this.PushButton, this.ClickAmount);
            }

            this.ResetPushState(null);
        }

        private void UpdateValue(UIImage image, float amount)
        {
            if (image == this.Minus)
            {
                this.Slider.Value -= amount;
            }
            else if (image == this.Plus)
            {
                this.Slider.Value += amount;
            }

        }

        private void OnSliderValueChanged(object sender, EventArgs e)
        {
            this.UpdateText();
        }

        private void UpdateText()
        {
            this.Text.Text.text = (this.Slider.Value * 100.0F).ToString("0") + "%";
        }

    }

}
