using DSharpPlus.Entities;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarkovLensBot.Functions;
using TarkovLensBot.Interfaces;

namespace TarkovLensBot.Helpers.ExtensionMethods
{
    static public class DiscordEmbedBuilderExtensions
    {
        public static DiscordEmbedBuilder AddAlternativeItemsFooter<T>(this DiscordEmbedBuilder embed, List<T> items, T chosenItem) 
            where T : IItem
        {
            var alternativeItems = items.Where(x => x.Name != chosenItem.Name).DistinctBy(x => x.Name).ToList();
            var alternativesString = alternativeItems.CreateAlternativesString();

            if (alternativeItems.IsNotNullOrEmpty())
            {
                embed.WithFooter($"Wrong item? Did you mean:{Environment.NewLine}{alternativesString}");
            }

            return embed;
        }
    }
}
