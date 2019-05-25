using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionDelegate : UIAction
    {
        public delegate bool ActDelegate(UIObject target, float delta);
        public delegate void BeginDelegate(UIObject target);
        public delegate void CompleteDelegate(UIObject target);

        public ActDelegate ActHandler { get; set; }
        public BeginDelegate BeginHandler { get; set; }
        public CompleteDelegate CompleteHandler { get; set; }

        public UIActionDelegate()
        {

        }

        protected override bool OnAct(UIObject target, float delta)
        {
            return this.ActHandler?.Invoke(target, delta) ?? base.OnAct(target, delta);
        }

        protected override void OnBegin(UIObject target)
        {
            base.OnBegin(target);

            this.BeginHandler?.Invoke(target);
        }

        protected override void OnComplete(UIObject target)
        {
            base.OnComplete(target);

            this.CompleteHandler?.Invoke(target);
        }

    }

}
