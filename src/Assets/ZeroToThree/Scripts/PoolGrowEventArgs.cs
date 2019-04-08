using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class PoolGrowEventArgs<T> : EventArgs
    {
        public T Obj { get; }
        public int Index { get; }

        public PoolGrowEventArgs(T obj, int index)
        {
            this.Obj = obj;
            this.Index = index;
        }

    }

}
