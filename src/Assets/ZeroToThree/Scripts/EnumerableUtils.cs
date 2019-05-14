using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public static class EnumerableUtils
    {
        public static void RemoveAll<V>(this ICollection<V> list, IEnumerable<V> enumerable)
        {
            foreach (var item in enumerable)
            {
                list.Remove(item);
            }

        }

    }

}
