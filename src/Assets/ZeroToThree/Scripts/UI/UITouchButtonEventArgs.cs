using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UITouchButtonEventArgs : UITouchEventArgs
    {
        public int Button { get; }

        public UITouchButtonEventArgs(Vector3 mousePosition, int button) : base(mousePosition)
        {
            this.Button = button;
        }

    }

}
