using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class YesNoDetermineEventArgs : EventArgs
    {
        public YesNoResult Result { get; private set; }

        public YesNoDetermineEventArgs(YesNoResult result)
        {
            this.Result = result;
        }

    }

}
