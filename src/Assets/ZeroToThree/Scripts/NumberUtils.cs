using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public static class NumberUtils
    {
        public static string ToNumberString(this IFormattable formattable)
        {
            return formattable.ToString("#,##0", null);
        }

    }

}
