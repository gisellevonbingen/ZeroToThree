using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public abstract class UIAction
    {
        public bool HasBegin { get; private set; }
        public bool Complete { get; private set; }

        public UIAction()
        {
            this.HasBegin = false;
            this.Complete = false;
        }

        public bool Act(UIObject target, float delta)
        {
            if (this.Complete == true)
            {
                return true;
            }
            else if (this.HasBegin == false)
            {
                this.HasBegin = true;
                this.OnBegin(target);
            }

            var complete = this.OnAct(target, delta);

            if (complete == true)
            {
                this.Complete = true;
                this.OnComplete(target);
            }

            return complete;
        }

        protected virtual bool OnAct(UIObject target, float delta)
        {
            return true;
        }

        protected virtual void OnBegin(UIObject target)
        {

        }

        protected virtual void OnComplete(UIObject target)
        {

        }

    }

}
