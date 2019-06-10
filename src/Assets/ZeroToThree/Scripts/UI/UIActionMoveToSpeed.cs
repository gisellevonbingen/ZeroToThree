using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionMoveToSpeed : UIAction
    {
        public Vector3 End { get; set; }
        public float Velocity { get; set; }
        public float Gain { get; set; }
        public float InPosition { get; set; }
        public float CurrentVelocity { get; private set; }

        public UIActionMoveToSpeed()
        {
            this.End = new Vector3();
            this.Velocity = 0.0F;
            this.Gain = 0.0F;
            this.InPosition = Physics2D.velocityThreshold;
        }

        protected override void OnBegin(UIObject target)
        {
            base.OnBegin(target);

            this.CurrentVelocity = this.Velocity;
        }

        protected override void OnComplete(UIObject target)
        {
            base.OnComplete(target);

            target.transform.localPosition = this.End;
        }

        protected override bool OnAct(UIObject target, float delta)
        {
            var end = this.End;
            var current = target.transform.localPosition;

            var deltaD = end - current;
            var normal = deltaD.normalized;

            if (deltaD.magnitude <= this.CurrentVelocity * delta)
            {
                this.CurrentVelocity *= this.Gain;
            }

            if (this.CurrentVelocity <= this.InPosition)
            {
                target.transform.localPosition = end;
                return true;
            }
            else
            {
                //var position = Vector3.MoveTowards(current, end, this.MaxDistanceDelta * delta);
                var position = current + (normal * this.CurrentVelocity * delta);

                target.transform.localPosition = position;

                if (position.sqrMagnitude == end.sqrMagnitude)
                {
                    return true;
                }

                return false;
            }

        }

    }

}
