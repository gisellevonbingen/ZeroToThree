using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIActionTimeDelegate : UIActionTime
    {
        public delegate void ActDelegate(UIObject target, float delta, float percent);
        public delegate void BeginDelegate(UIObject target);
        public delegate void CompleteDelegate(UIObject target);

        public ActDelegate ActHandler { get; set; }
        public BeginDelegate BeginHandler { get; set; }
        public CompleteDelegate CompleteHandler { get; set; }

        public UIActionTimeDelegate()
        {

        }

        protected override void OnAct(UIObject target, float delta, float percent)
        {
            this.ActHandler?.Invoke(target, delta, percent);
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
