using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionScaleToTime : UIActionTime
    {
        private Vector3 Start;

        public Vector3 End { get; set; }

        public UIActionScaleToTime()
        {
            this.Start = new Vector3();
            this.End = new Vector3();
        }

        protected override void OnAct(UIObject target, float delta, float percent)
        {
            base.OnAct(target, delta, percent);

            var scale = Vector3.Lerp(this.Start, this.End, percent);
            target.transform.localScale = scale;
        }

        protected override void OnBegin(UIObject target)
        {
            base.OnBegin(target);

            this.Start = target.transform.localScale;
        }

        protected override void OnComplete(UIObject target)
        {
            base.OnComplete(target);

            target.transform.localScale = this.End;
        }

    }

}
