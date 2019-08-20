using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIVolumeControl : UIObject
    {
        public UIImage Minus;
        public UIImage Plus;
        public UISlider Slider;
        public UILabel Text;

        public float ButtonAmount;

        protected override void Awake()
        {
            base.Awake();

            this.Minus.TouchButtonClick += this.OnMinusTouchButtonClick;
            this.Plus.TouchButtonClick += this.OnPlusTouchButtonClick;
            this.Slider.ValueChanged += this.OnSliderValueChanged;

            this.UpdateText();
        }

        private void OnMinusTouchButtonClick(object sender, UITouchButtonEventArgs e)
        {
            this.Slider.Value -= this.ButtonAmount;
        }

        private void OnPlusTouchButtonClick(object sender, UITouchButtonEventArgs e)
        {
            this.Slider.Value += this.ButtonAmount;
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
