using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class UpdateArgs
    {
        public Vector2 MousePosition { get; private set; }

        public UpdateArgs(Vector2 mousePosition)
        {
            this.MousePosition = mousePosition;
        }

    }

}
