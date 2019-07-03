using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UICommonButton : UIObject
    {
        public UIImage Image;
        public UILabel Label;

        protected override void Awake()
        {
            base.Awake();

            this.Image.Click += this.OnImageClick;
        }

        private void OnImageClick(object sender, UIClickEventArgs e)
        {
            this.PerformClick(e);
        }

    }

}
