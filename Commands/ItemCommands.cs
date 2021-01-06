using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsciiTableFormatter;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using TarkovLensBot.Helpers.ExtensionMethods;
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
        public async Task GetItemPrice(CommandContext ctx, [Description("The name of the item to find, e.g. salewa.")] params string[] input)
        {
            var name = input.ToStringWithSpaces();

            var items = await _tarkovLensService.GetItemsBySearch(name).ConfigureAwait(false);
            var item = items.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();

            if (item.IsNull())
                item = items.FirstOrDefault();

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
        public async Task CompareAmmo(
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
            return;
        }

        [Command("ammo")]
        [Description("Get information about an ammunition")]
        public async Task GetAmmoInfo(
            CommandContext ctx, 
            [Description("Optional: The caliber of the ammo (useful if there are many types of ammunition with the same name)")] string caliber = null,
            [Description("The name of the ammo")] params string[] input)
        {
            var name = input.ToStringWithSpaces();
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(nameOfItem: name, caliber: caliber);
            Ammunition ammo = null;

            // Some filtering to more accurately choose the ammo that the user searched for
            foreach (var a in ammunitions)
            {
                if (a.Name.ContainsWord(name))
                {
                    ammo = a;
                    break;
                }
            }

            // If there is no direct word match, use the first item that the API returned
            if (ammo.IsNull())
                ammo = ammunitions.FirstOrDefault();

            // If still no ammo, return "not found" message
            if (ammo.IsNull())
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
                Title = $"{ammo.Name}",
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

        [Command("caliber")]
        [Description("Get information about a caliber")]
        public async Task GetCaliberInfo(CommandContext ctx, [Description("The name of the caliber")] string caliberInput)
        {
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(caliber: caliberInput);

            #region Precise filtering
            // Some filtering to more accurately choose the ammo that the user searched for
            var ammunitionsFiltered = new List<Ammunition>();
            foreach (var ammo in ammunitions)
            {
                if (ammo.Caliber.ContainsWord(caliberInput))
                {
                    ammunitionsFiltered.Add(ammo);
                }
            }

            if (ammunitionsFiltered.IsNotNullOrEmpty())
            {
                ammunitions = ammunitionsFiltered;
            }

            // If still no ammo, return "not found" message
            if (ammunitions.IsNullOrEmpty())
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "No ammunition found for this caliber",
                    Description = caliberInput,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }
            #endregion

            #region Create display table
            var comparisonList = new List<CaliberComparisonItem>();

            foreach (var ammo in ammunitions)
            {
                comparisonList.Add(new CaliberComparisonItem(ammo.ShortName, ammo.Caliber, ammo.Damage, ammo.Penetration, ammo.ArmorDamage, ammo.Velocity, ammo.Tracer));
            }
            comparisonList = comparisonList.OrderBy(x => x.Name).ToList();

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";
            #endregion

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
            return;
        }
    }
}
