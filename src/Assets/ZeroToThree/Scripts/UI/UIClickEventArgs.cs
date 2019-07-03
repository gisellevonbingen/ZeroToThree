using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIClickEventArgs : UIEventArgs
    {
        public Vector3 MousePosition { get; }
        public int Button { get; }

        public UIClickEventArgs(Vector3 mousePosition, int button) : base()
        {
            this.MousePosition = mousePosition;
            this.Button = button;
        }

    }

}
