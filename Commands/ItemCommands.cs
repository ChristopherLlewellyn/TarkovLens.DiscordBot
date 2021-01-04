using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsciiTableFormatter;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using TarkovLensBot.Models.CommandResponses;
using TarkovLensBot.Models.Items;
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

        [Command("price")]
        [Description("Gets the market price of an item")]
        public async Task GetItemPrice(CommandContext ctx, [Description("The name of the item to find, e.g. salewa.")] string name)
        {
            var items = await _tarkovLensService.GetItemsBySearch(name).ConfigureAwait(false);
            var item = items.FirstOrDefault();

            if (item != null)
            {
                var msgEmbed = new DiscordEmbedBuilder
                {
                    Title = item.Name,
                    ImageUrl = item.Img,
                    Color = DiscordColor.Orange
                };

                msgEmbed.AddField("Market Price", $"{item.LastLowestMarketPrice} ₽");

                await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
                return;
            }
            else
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Item not found",
                    Description = name,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }
        }

        [Command("compare")]
        [Description("Compare two different ammunitions")]
        public async Task GetAmmoInfo(
            CommandContext ctx,
            [Description("The name of the first ammo to compare, e.g. m995")] string ammo1Name,
            [Description("The name of the second ammo to compare, e.g. BT")] string ammo2Name
            )
        {
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions();

            var ammo1 = ammunitions.Where(x => x.Name.ToLower().Contains(ammo1Name.ToLower())).FirstOrDefault();
            var ammo2 = ammunitions.Where(x => x.Name.ToLower().Contains(ammo2Name.ToLower())).FirstOrDefault();

            if (ammo1 == null || ammo2 == null)
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Item(s) not found",
                    Color = DiscordColor.Red
                };

                if (ammo1 == null)
                    errEmbed.AddField("Could not find any items matching:", ammo1Name);
                if (ammo2 == null)
                    errEmbed.AddField("Could not find any items matching:", ammo2Name);

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var comparisonList = new List<AmmoComparisonItem>();

            var ammo1Details = new AmmoComparisonItem(ammo1.ShortName, ammo1.Caliber, ammo1.Damage, ammo1.Penetration, ammo1.ArmorDamage, ammo1.Velocity, ammo1.Tracer);
            var ammo2Details = new AmmoComparisonItem(ammo2.ShortName, ammo2.Caliber, ammo2.Damage, ammo2.Penetration, ammo2.ArmorDamage, ammo2.Velocity, ammo2.Tracer);

            comparisonList.Add(ammo1Details);
            comparisonList.Add(ammo2Details);

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
        }

        [Command("ammo")]
        [Description("Get information about an ammunition")]
        public async Task GetAmmoInfo(CommandContext ctx, [Description("The name of the ammo")] string name)
        {
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(name);

            var ammo = ammunitions.FirstOrDefault();

            if (ammo == null)
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Item not found",
                    Description = name,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var msgEmbed = new DiscordEmbedBuilder
            {
                Title = $"Stats for {ammo.Name}",
                Color = DiscordColor.Teal
            };

            msgEmbed.AddField("Damage", ammo.Damage.ToString());
            msgEmbed.AddField("Penetration", ammo.Penetration.ToString());
            msgEmbed.AddField("Armor Damage", ammo.ArmorDamage.ToString());
            msgEmbed.AddField("Velocity", ammo.Velocity.ToString());
            msgEmbed.AddField("Tracer?", ammo.Tracer == true ? "Yes" : "No");
            msgEmbed.AddField("Caliber", ammo.Caliber.ToString());

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }
    }
}
