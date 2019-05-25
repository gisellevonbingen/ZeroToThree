using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionRotateToTime : UIActionTime
    {
        private float Start;
        public float End { get; set; }

        public UIActionRotateToTime()
        {
            this.Start = 0.0F;
            this.End = 0.0F;
        }

        protected override void OnBegin(UIObject target)
        {
            base.OnBegin(target);

            this.Start = this.GetRotation(target);
        }

        protected override void OnAct(UIObject target, float delta, float percent)
        {
            base.OnAct(target, delta, percent);

            var rotation = Mathf.Lerp(this.Start, this.End, percent);
            this.SetRotation(target, rotation);
        }

        protected override void OnComplete(UIObject target)
        {
            base.OnComplete(target);

            this.SetRotation(target, this.End);
        }

        public float GetRotation(UIObject target)
        {
            return target.transform.localRotation.eulerAngles.z;
        }

        public void SetRotation(UIObject target, float rotate)
        {
            var rotation = target.transform.localRotation;
            var angles = rotation.eulerAngles;
            angles.z = rotate;
            rotation.eulerAngles = angles;

            target.transform.localRotation = rotation;
        }

    }

}
