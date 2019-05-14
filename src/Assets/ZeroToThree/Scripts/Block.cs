using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class Block
    {
        public int Value { get; set; }
        public bool Masking { get; set; }

        public Block()
        {

        }

        public Block(Block other)
        {
            this.Value = other.Value;
            this.Masking = other.Masking;
        }

        public Block Clone()
        {
            return new Block(this);
        }

    }

}
