using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarkovLensBot.Interfaces;

namespace TarkovLensBot.Functions
{
    public static class MessageFunctions
    {
        public static string CreateAlternativesString<T>(this List<T> items) where T : IItem
        {
            var maxAlternatives = 3;
            var returnString = string.Empty;
            for (var i = 0; i < items.Count; i++)
            {
                if (i != 0)
                {
                    returnString += Environment.NewLine;
                }

                returnString += $"\"{items[i].Name}\"";

                if (i == maxAlternatives - 1) // Max 3 alternatives
                {
                    if (items.Count > maxAlternatives)
                    {
                        returnString += Environment.NewLine;
                        returnString += $"...and {items.Count - maxAlternatives} more items";
                    }
                    break;
                }
            }

            return returnString;
        }
    }
}
