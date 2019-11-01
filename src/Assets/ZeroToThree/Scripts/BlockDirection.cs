using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class BlockDirection : IEquatable<BlockDirection>
    {
        private static readonly List<BlockDirection> List = new List<BlockDirection>();
        public static BlockDirection[] Values => List.ToArray();

        public static BlockDirection Left { get; } = new BlockDirection(nameof(Left), -1, 0, () => Right);
        public static BlockDirection Right { get; } = new BlockDirection(nameof(Right), +1, 0, () => Left);
        public static BlockDirection Up { get; } = new BlockDirection(nameof(Up), 0, -1, () => Down);
        public static BlockDirection Down { get; } = new BlockDirection(nameof(Down), 0, +1, () => Up);

        public string Name { get; }
        public int X { get; }
        public int Y { get; }

        private readonly Func<BlockDirection> OppositeFunc;

        private BlockDirection(string name, int x, int y, Func<BlockDirection> opposite)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.OppositeFunc = opposite;

            List.Add(this);
        }

        public BlockDirection Opposite => this.OppositeFunc();

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        public bool Equals(BlockDirection other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + this.X;
            hash = hash * 31 + this.Y;

            return hash;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }

}
