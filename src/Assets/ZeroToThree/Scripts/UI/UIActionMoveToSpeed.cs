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
        public float MaxDistanceDelta { get; set; }

        public UIActionMoveToSpeed()
        {
            this.End = new Vector3();
            this.MaxDistanceDelta = 0.0F;
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
            var position = Vector3.MoveTowards(current, end, this.MaxDistanceDelta * delta);
            target.transform.localPosition = position;

            if (position.sqrMagnitude == end.sqrMagnitude)
            {
                return true;
            }

            return false;
        }

    }

}
