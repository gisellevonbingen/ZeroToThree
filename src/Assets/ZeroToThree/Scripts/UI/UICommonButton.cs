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

        protected override void OnAwake()
        {
            base.OnAwake();

            this.Image.TouchButtonClick += this.OnImageClick;
        }

        private void OnImageClick(object sender, UITouchButtonEventArgs e)
        {
            this.PerformTouchButtonClick(e);
        }

    }

}
