using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TarkovLensBot.Helpers.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static string ToTitleCase(this string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static bool ContainsWord(this string s, string word)
        {
            string[] ar = s.Split(' ');

            foreach (string str in ar)
            {
                if (str.ToLower() == word.ToLower())
                    return true;
            }
            return false;
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static bool IsTooShortForSearching(this string str, int minQueryLength)
        {
            return str.Length < minQueryLength ? true : false;
        }

        public static string AddMentions(this string str, IEnumerable<string> mentions, bool newLine)
        {
            if (mentions.IsNotNullOrEmpty())
            {
                str += newLine ? Environment.NewLine : "";
                foreach (var mention in mentions)
                {
                    str += $"{mention} ";
                }
            }
            return str;
        }

        public static string AddMention(this string str, string mention, bool newLine)
        {
            return AddMentions(str, new List<string> { mention }, newLine);
        }
    }
}
