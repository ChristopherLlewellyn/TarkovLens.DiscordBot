using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TarkovLensBot.Helpers.ExtensionMethods
{
    public static class NumberExtensions
    {
        public static bool IsEven(this int number)
        {
            if (number % 2 == 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsOdd(this int number)
        {
            if (number % 2 != 0)
            {
                return true;
            }
            return false;
        }
    }
}
