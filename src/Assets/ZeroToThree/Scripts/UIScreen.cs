using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class UIScreen : UIObject
    {
        public void OnOpen()
        {
            this.gameObject.SetActive(true);
        }

        public void OnClose()
        {
            this.gameObject.SetActive(false);
        }

    }

}
