using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UITouchEventArgs : UIEventArgs
    {
        public Vector3 MousePosition { get; }

        public UITouchEventArgs(Vector3 mousePosition) : base()
        {
            this.MousePosition = mousePosition;
        }

    }

}
