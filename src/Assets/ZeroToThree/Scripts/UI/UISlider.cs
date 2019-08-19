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
        public UIImage Back;
        public UIImage Handle;

        private float _Value;
        public float Value { get => this._Value; set { this._Value = value; this.OnValueChagned(EventArgs.Empty); } }

        private bool Handling;

        protected override void Awake()
        {
            base.Awake();

            this.Value = 0.0F;

            this.Handling = false;

            this.Handle.TouchButtonDown += this.OnTouchButtonDown;
            this.Handle.TouchButtonUp += this.OnTouchButtonUp;
            this.Back.TouchButtonDown += this.OnTouchButtonDown;
            this.Back.TouchButtonUp += this.OnTouchButtonUp;
        }

        protected virtual void OnValueChagned(EventArgs e)
        {
            var handle = this.Handle;
            var width = this.Back.transform.rect.width;
            var handlePosition = handle.transform.localPosition;

            handlePosition.x = this.Value - width / 2.0F;
            handle.transform.localPosition = handlePosition;
        }

        protected override void Update()
        {
            base.Update();

            if (this.Handling == true)
            {
                this.UpdateValue();
            }

        }

        private void UpdateValue()
        {
            var center = this.transform.position;
            var width = this.Back.transform.rect.width;

            var minPosition = new Vector3(center.x - width / 2.0F, center.y, center.z);
            var mousePosition = GameManager.Instance.UIManager.MousePosition;
            var value = Mathf.Clamp(mousePosition.x - minPosition.x, 0.0F, width);

            this.Value = value;
        }

        private void OnTouchButtonDown(object sender, UITouchButtonEventArgs e)
        {
            this.Handling = true;
            this.UpdateValue();
        }

        private void OnTouchButtonUp(object sender, UITouchButtonEventArgs e)
        {
            this.Handling = false;
        }

    }

}
