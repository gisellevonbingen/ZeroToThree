using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public static class NumberUtils
    {
        public static bool IsGeneral(this float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }

            return true;
        }

        public static string ToNumberString(this IFormattable formattable)
        {
            return formattable.ToString("#,##0", null);
        }

    }

}
