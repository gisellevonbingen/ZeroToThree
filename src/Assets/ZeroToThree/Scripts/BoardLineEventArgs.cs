using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class BoardLineEventArgs : EventArgs
    {
        public int Lines { get; }
        public Block[] Blocks { get; }

        public BoardLineEventArgs(int lines, Block[] blocks)
        {
            this.Lines = lines;
            this.Blocks = blocks;
        }

    }

}
