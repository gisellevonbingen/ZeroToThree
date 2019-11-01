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
        public EventPhase Phase { get; }

        public BlocksEventArgs(Block[] blocks, EventPhase phase)
        {
            this.Blocks = blocks;
            this.Phase = phase;
        }

        public enum EventPhase : byte
        {
            Pre = 0,
            Post = 1,
        }

    }

}
