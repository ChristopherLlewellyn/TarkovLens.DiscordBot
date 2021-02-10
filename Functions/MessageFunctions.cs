using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarkovLensBot.Helpers.ExtensionMethods;
using TarkovLensBot.Interfaces;
using TarkovLensBot.Models.Items;

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

                returnString += $"• {items[i].Name}";

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

        public static List<Ammunition> ParseCompareAmmoInput(List<Ammunition> allAmmunitions, string[] input)
        {
            var foundAmmos = new List<Ammunition>();
            var currentCaliber = string.Empty;
            var currentAmmoName = string.Empty;

            // Check each word in the input, extract the calibers and the ammo names
            for (var i = 0; i < input.Length; i++)
            {
                // Even numbers are a caliber
                if (i.IsEven())
                {
                    currentCaliber = input[i];
                }

                // Odd numbers are the name of an ammunition
                else
                {
                    currentAmmoName = input[i].Replace(",", ""); // Remove comma separator if it exists

                    // Now we should have both a caliber and an ammo name, so we can start searching for the ammunition
                    Ammunition ammo = allAmmunitions
                        .Where(x => x.Caliber.ToLower().Contains(currentCaliber.ToLower())
                            && x.Name.ToLower().Contains(currentAmmoName.ToLower()))
                        .FirstOrDefault();

                    if (ammo.IsNotNull())
                    {
                        foundAmmos.Add(ammo);
                    }
                }
            }

            return foundAmmos;
        }
    }
}
