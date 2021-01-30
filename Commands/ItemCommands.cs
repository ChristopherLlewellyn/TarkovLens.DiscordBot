using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsciiTableFormatter;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MoreLinq;
using TarkovLensBot.Functions;
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
        public async Task GetItemPrice(CommandContext ctx, [Description("The name of the item to find, e.g. salewa")] params string[] name)
        {
            var nameString = name.ToStringWithSpaces();

            var items = await _tarkovLensService.GetItemsBySearch(nameString).ConfigureAwait(false);
            items = items
                .Where(x => x.Avg24hPrice > 0)
                .OrderBy(x => x.Name.Length)
                .ToList();

            var item = items.Where(x => x.Name.ToLower() == nameString.ToLower()).FirstOrDefault();

            if (item.IsNull())
                item = items.Where(x => x.Name.ToLower().Contains(nameString.ToLower())).FirstOrDefault();

            if (item.IsNull())
                item = items.FirstOrDefault();

            // Alternative items that closely matched the user's input
            var alternatives = items.Where(x => x.Name != item.Name).DistinctBy(x => x.Name).ToList();
            var alternativesString = alternatives.CreateAlternativesString();

            // Build and return response message
            var responseMsg = new DiscordEmbedBuilder();

            if (item != null)
            {
                responseMsg = new DiscordEmbedBuilder
                {
                    Title = $"{item.Avg24hPrice} ₽",
                    ImageUrl = item.Img,
                    Color = DiscordColor.Orange
                };

                responseMsg.AddField("Item", item.Name);

                if (alternatives.IsNotNullOrEmpty())
                {
                    responseMsg.WithFooter($"Wrong item? Did you mean:{Environment.NewLine}{Environment.NewLine}{alternativesString}");
                }

                await ctx.Channel.SendMessageAsync(embed: responseMsg).ConfigureAwait(false);
                return;
            }
            
            if (item.IsNull())
            {
                responseMsg = new DiscordEmbedBuilder
                {
                    Title = "Item not found",
                    Description = $"\"{nameString}\"",
                    Color = DiscordColor.Red
                };

                responseMsg.WithFooter("Note: search using the long name, e.g. instead of \"btc\" use \"bitcoin\"");

                await ctx.Channel.SendMessageAsync(embed: responseMsg).ConfigureAwait(false);
                return;
            }
        }

        [Command("ammocompare")]
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

            var ammo1Details = new AmmoComparisonItem(ammo1.ShortName, ammo1.Caliber, ammo1.Damage, ammo1.Penetration, ammo1.ArmorDamage, ammo1.Velocity, ammo1.Tracer, ammo1.Avg24hPrice);
            var ammo2Details = new AmmoComparisonItem(ammo2.ShortName, ammo2.Caliber, ammo2.Damage, ammo2.Penetration, ammo2.ArmorDamage, ammo2.Velocity, ammo2.Tracer, ammo2.Avg24hPrice);

            comparisonList.Add(ammo1Details);
            comparisonList.Add(ammo2Details);

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
            return;
        }

        [Command("ammo")]
        [Description("Get information about an ammunition type")]
        public async Task GetAmmoInfo(
            CommandContext ctx, 
            [Description("The caliber of the ammo")] string caliber = null,
            [Description("The name of the ammo")] params string[] name)
        {
            var nameString = name.ToStringWithSpaces();
            if (nameString.IsNullOrEmpty()) // If name is empty, then they only entered one parameter. So the caliber IS the name.
            {
                nameString = caliber;
                caliber = null;
            }

            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(nameOfItem: nameString, caliber: caliber);
            Ammunition ammo = null;

            // Some filtering to more accurately choose the ammo that the user searched for
            foreach (var a in ammunitions)
            {
                if (a.Name.ContainsWord(nameString))
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
                    Description = nameString,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var msgEmbed = new DiscordEmbedBuilder
            {
                Title = ammo.Name,
                ImageUrl = ammo.Img,
                Color = DiscordColor.Teal
            };

            msgEmbed.AddField("Damage", ammo.Damage.ToString());
            msgEmbed.AddField("Penetration", ammo.Penetration.ToString());
            msgEmbed.AddField("Armor Damage", ammo.ArmorDamage.ToString());
            msgEmbed.AddField("Velocity", ammo.Velocity.ToString());
            msgEmbed.AddField("Tracer?", ammo.Tracer == true ? "Yes" : "No");
            msgEmbed.AddField("Caliber", ammo.Caliber.ToString());
            msgEmbed.AddField("Market Price", $"{ammo.Avg24hPrice.ToString()} ₽");

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }

        [Command("caliber")]
        [Description("Get information about a caliber")]
        public async Task GetCaliberInfo(CommandContext ctx, [Description("The name of the caliber")] params string[] caliber)
        {
            string caliberString = caliber.ToStringWithSpaces();
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(caliber: caliberString);

            #region Precise filtering
            // Some filtering to more accurately choose the ammo that the user searched for
            var ammunitionsFiltered = new List<Ammunition>();
            foreach (var ammo in ammunitions)
            {
                if (ammo.Caliber.ContainsWord(caliberString))
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
                    Description = caliberString,
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
                comparisonList.Add(new CaliberComparisonItem(ammo.ShortName, ammo.Caliber, ammo.Damage, ammo.Penetration, ammo.ArmorDamage, ammo.Velocity, ammo.Tracer, ammo.Avg24hPrice));
            }
            comparisonList = comparisonList.OrderBy(x => x.Name).ToList();

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";
            #endregion

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
            return;
        }

        [Command("armor")]
        [Description("Get information about an armor")]
        public async Task GetArmorInfo(CommandContext ctx, [Description("The name of the armor")] params string[] name)
        {
            string nameString = name.ToStringWithSpaces().ToLower();
            List<Armor> armors = await _tarkovLensService.GetItemsByKind<Armor>(Enums.KindOfItem.Armor);

            var armor = armors.Where(x => x.Name.ToLower().Contains(nameString)).FirstOrDefault();

            if (armor.IsNull())
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Armor not found",
                    Description = nameString,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var msgEmbed = new DiscordEmbedBuilder
            {
                Title = armor.Name,
                ImageUrl = armor.Img,
                Color = DiscordColor.Teal
            };

            msgEmbed.AddField("Class", armor.ArmorProperties.Class.ToString());
            msgEmbed.AddField("Max Durability", armor.ArmorProperties.Durability.ToString());
            msgEmbed.AddField("Protects", armor.ArmorProperties.Zones.Join(", "));
            msgEmbed.AddField("Material", armor.ArmorProperties.Material.Name.ToString());
            msgEmbed.AddField("Weight", $"{armor.Weight.ToString()} kg");
            msgEmbed.AddField("Movement Speed", $"{armor.Penalties.Speed.ToString()}%");
            msgEmbed.AddField("Ergonomics", $"{armor.Penalties.Ergonomics.ToString()}%");
            msgEmbed.AddField("Turn Speed", $"{armor.Penalties.Mouse.ToString()}%");
            msgEmbed.AddField("Market Price", $"{armor.Avg24hPrice.ToString()} ₽");

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }

        [Command("medical")]
        [Description("Get information about a medical item")]
        public async Task GetMedicalInfo(CommandContext ctx, [Description("The name of the medical item")] params string[] name)
        {
            string nameString = name.ToStringWithSpaces().ToLower();
            List<Medical> medicals = await _tarkovLensService.GetItemsByKind<Medical>(Enums.KindOfItem.Medical);

            var medical = medicals.Where(x => x.Name.ToLower().Contains(nameString)).FirstOrDefault();

            if (medical.IsNull())
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Medical item not found",
                    Description = nameString,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var msgEmbed = new DiscordEmbedBuilder
            {
                Title = medical.Name,
                ImageUrl = medical.Img,
                Color = DiscordColor.Teal
            };

            if (medical.Resources.IsNotNull() && medical.Resources != 0)
            {
                msgEmbed.AddField("Max resources", medical.Resources.ToString());
            }

            if (medical.UseTime.IsNotNull())
            {
                msgEmbed.AddField("Use time", $"{medical.UseTime.ToString()} sec");
            }

            if (medical.Effects.Health.IsNotNull())
            {
                msgEmbed.AddField("Amount healed", $"{medical.Effects.Health.Value.ToString()}");
            }

            if (medical.Effects.Energy.IsNotNull())
            {
                if (medical.Effects.Energy.Value.IsNotNull())
                    msgEmbed.AddField("Energy", $"{medical.Effects.Energy.Value}");
            }

            if (medical.Effects.Hydration.IsNotNull())
            {
                if (medical.Effects.Hydration.Value.IsNotNull())
                    msgEmbed.AddField("Hydration", $"{medical.Effects.Hydration.Value}");
            }

            if (medical.Effects.Bloodloss.IsNotNull())
            {
                if (medical.Effects.Bloodloss.Removes.IsNotNull() && medical.Effects.Bloodloss.Removes == true) 
                    msgEmbed.AddField("Removes bloodloss?", "Yes");
            }

            if (medical.Effects.LightBleeding.IsNotNull())
            {
                if (medical.Effects.LightBleeding.Removes.IsNotNull() && medical.Effects.LightBleeding.Removes == true)
                    msgEmbed.AddField("Removes light bleeding?", "Yes");
            }

            if (medical.Effects.HeavyBleeding.IsNotNull())
            {
                if (medical.Effects.HeavyBleeding.Removes.IsNotNull() && medical.Effects.HeavyBleeding.Removes == true)
                    msgEmbed.AddField("Removes heavy bleeding?", "Yes");
            }

            if (medical.Effects.Fracture.IsNotNull())
            {
                if (medical.Effects.Fracture.Removes.IsNotNull() && medical.Effects.Fracture.Removes == true)
                    msgEmbed.AddField("Removes fractures?", "Yes");
            }

            if (medical.Effects.Pain.IsNotNull())
            {
                if (medical.Effects.Pain.Removes.IsNotNull() && medical.Effects.Pain.Removes == true)
                    msgEmbed.AddField("Removes pain?", $"Yes, for {medical.Effects.Pain.Duration} sec");
            }

            if (medical.Effects.Contusion.IsNotNull())
            {
                if (medical.Effects.Contusion.Removes.IsNotNull() && medical.Effects.Contusion.Removes == true)
                    msgEmbed.AddField("Removes contusion?", "Yes");
            }

            if (medical.Effects.Tremor.IsNotNull())
            {
                if (medical.Effects.Tremor.Removes.IsNotNull() && medical.Effects.Tremor.Removes == false && medical.Effects.Tremor.Chance == 1)
                {
                    var tremorInfo = $"Yes, for {medical.Effects.Tremor.Duration} sec";
                    tremorInfo += medical.Effects.Tremor.Delay > 0 ? $" after a {medical.Effects.Tremor.Delay} sec delay" : "";
                    msgEmbed.AddField("Causes tremor?", tremorInfo);
                }
            }

            msgEmbed.AddField("Market Price", $"{medical.Avg24hPrice} ₽");

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }
    }
}
