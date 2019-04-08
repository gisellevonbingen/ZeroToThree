using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class BlocksEventArgs : EventArgs
    {
        public Block[] Blocks { get; }

        public BlocksEventArgs(Block[] blocks)
        {
            this.Blocks = blocks;
        }

    }

}
