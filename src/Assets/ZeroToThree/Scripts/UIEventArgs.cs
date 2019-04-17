using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class UIEventArgs : EventArgs
    {
        public UIObject Source { get; private set;}

        public UIEventArgs(UIObject source)
        {
            this.Source = source;
        }

    }

}
