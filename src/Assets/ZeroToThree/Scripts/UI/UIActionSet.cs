using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionSet : UIAction
    {
        public List<UIAction> Actions { get; private set; }

        public UIActionSet()
        {
            this.Actions = new List<UIAction>();
        }

        protected override bool OnAct(UIObject target, float delta)
        {
            var result = true;

            foreach (var action in this.Actions)
            {
                if (action.Act(target, delta) == false)
                {
                    result = false;
                }

            }

            return result;
        }

    }

}
