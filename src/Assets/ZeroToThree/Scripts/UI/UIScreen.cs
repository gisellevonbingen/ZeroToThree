using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIScreen : UIObject
    {
        public virtual void OnOpen()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void OnClose()
        {
            this.gameObject.SetActive(false);
        }

    }

}
