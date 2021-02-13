using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarkovLensBot.Helpers.ExtensionMethods;

namespace TarkovLensBot.Models.CommandResponses
{
    public class QueryTooShortResponse
    {
        public DiscordEmbedBuilder Message { get; set; }
        public QueryTooShortResponse(string query, int minQueryLength, string mention = null)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Provided query is too short",
                Description = $"Minimum length is {minQueryLength} characters. ".AddMention(mention, newLine: false),
                Color = DiscordColor.Red
            };
            Message = builder;
        }
    }
}
