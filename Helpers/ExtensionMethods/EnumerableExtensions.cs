using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TarkovLensBot.Helpers.ExtensionMethods
{
    static public class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            return !enumerable.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !IsNullOrEmpty(enumerable);
        }

        public static string ToStringWithSpaces(this IEnumerable<string> enumerable)
        {
            string output = string.Empty;
            foreach (var item in enumerable) output += item + " ";
            return output.Trim();
        }
    }
}
