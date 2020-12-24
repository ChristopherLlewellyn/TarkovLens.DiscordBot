using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using TarkovLensBot.Services;

namespace TarkovLensBot.Commands
{
    public class ItemCommands : BaseCommandModule
    {
        private readonly TarkovLensService _tarkovLensService;

        public ItemCommands(TarkovLensService tarkovLensService)
        {
            _tarkovLensService = tarkovLensService;
        }

        [Command("name")]
        [Description("Returns your name")]
        public async Task Ping(CommandContext ctx, [Description("Your name")] string name)
        {
            await ctx.Channel.SendMessageAsync(name).ConfigureAwait(false);
        }

        [Command("p")]
        [Description("Gets the market price of an item")]
        public async Task GetItemPrice(CommandContext ctx, [Description("The name of the item to find, e.g. salewa.")] string name)
        {
            var items = await _tarkovLensService.GetItemsBySearch(name).ConfigureAwait(false);
            var item = items.FirstOrDefault();

            if (item != null)
            {
                await ctx.Channel.SendMessageAsync($"\"{item.Name}\": {item.LastLowestMarketPrice} ₽").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Could not find any items matching \"{name}\"").ConfigureAwait(false);
            }
        }
    }
}
