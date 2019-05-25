using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionTime : UIAction
    {
        public float Elapsed { get; set; }
        public float Duration { get; set; }

        public UIActionTime()
        {
            this.Elapsed = 0.0F;
            this.Duration = 0.0F;
        }

        protected override void OnBegin(UIObject target)
        {
            base.OnBegin(target);

            this.Elapsed = 0.0F;
        }

        protected override bool OnAct(UIObject target, float delta)
        {
            base.OnAct(target, delta);

            this.Elapsed += delta;

            var elapsed = this.Elapsed;
            var duration = this.Duration;

            if (elapsed >= duration)
            {
                return true;
            }
            else
            {
                var percent = elapsed / duration;
                this.OnAct(target, delta, percent);
                return false;
            }

        }

        protected virtual void OnAct(UIObject target, float delta, float percent)
        {

        }

    }

}
