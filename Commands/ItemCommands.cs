using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsciiTableFormatter;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
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
        [Aliases("p")]
        [Description("Gets the market price of an item. Example usage: \"!price salewa\"")]
        public async Task GetItemPrice(CommandContext ctx, [Description("The name of the item to find, e.g. salewa")] params string[] name)
        {
            var nameString = name.ToStringWithSpaces();

            var items = await _tarkovLensService.GetItemsBySearch(nameString).ConfigureAwait(false);
            items = items.Where(x => x.Avg24hPrice > 0).ToList(); // Removes stuff like quest items, that don't have value
            var item = items.SearchForItem(nameString);

            // Build and return response message
            var responseMsg = new DiscordEmbedBuilder();

            if (item != null)
            {
                responseMsg = new DiscordEmbedBuilder 
                { 
                    Color = DiscordColor.Orange,
                    ImageUrl = item.Img
                };
                responseMsg.AddField($"{item.Avg24hPriceFormatted}   |", "`Total`", true);
                if (item.Avg24hPricePerSlot.IsNotNull())
                {
                    responseMsg.AddField(item.Avg24hPricePerSlotFormatted, $"`Per slot (x{item.Grid.Height * item.Grid.Width})`", true);
                }
                responseMsg.AddAlternativeItemsFooter(items, item);

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

        [Command("compareammo")]
        [Aliases("cammo")]
        [Description("Compare multiple ammunitions. Example usage: \"!compareammo 5.56 m995, 5.45 bt, 9x19 ap\"")]
        public async Task CompareAmmo(
            CommandContext ctx,
            [Description("A string of caliber and ammo names")] params string[] input
            )
        {
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions();
            var ammosToCompare = MessageFunctions.ParseCompareAmmoInput(ammunitions, input);

            if (ammosToCompare.IsNullOrEmpty())
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Item(s) not found",
                    Color = DiscordColor.Red
                };
                errEmbed.AddField("Tip", "Provide the caliber and the name of the ammunition", inline: true);
                errEmbed.AddField("Example", "\"5.56 m995, 5.45 bt, 9x19 ap\"", inline: true);

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var comparisonList = new List<AmmoComparisonItem>();
            foreach (var ammo in ammosToCompare)
            {
                var tableRow = new AmmoComparisonItem(ammo.ShortName, ammo.Caliber, ammo.Damage, ammo.Penetration, ammo.ArmorDamage, ammo.Velocity, ammo.Tracer, ammo.Avg24hPrice);
                comparisonList.Add(tableRow);
            }

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
            return;
        }

        [Command("ammo")]
        [Description("Get information about an ammunition type. Example usage: \"!ammo 5.56 m995\"")]
        public async Task GetAmmoInfo(
            CommandContext ctx, 
            [Description("The caliber of the ammo")] string caliber,
            [Description("The name of the ammo")] params string[] name)
        {
            var nameString = name.ToStringWithSpaces();
            if (nameString.IsNullOrEmpty()) // If name is empty, then they only entered one parameter. So the caliber IS the name.
            {
                nameString = caliber;
                caliber = null;
            }

            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(nameOfItem: nameString, caliber: caliber);
            Ammunition ammo = ammunitions.SearchForItem(nameString);

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
                Color = DiscordColor.Teal
            };
            msgEmbed.WithThumbnail(ammo.Img);
            msgEmbed.AddField("Damage", ammo.Damage.ToString(), true);
            msgEmbed.AddField("Penetration", ammo.Penetration.ToString(), true);
            msgEmbed.AddField("Armor Damage", ammo.ArmorDamage.ToString(), true);
            msgEmbed.AddField("Velocity", ammo.VelocityFormatted, true);
            msgEmbed.AddField("Tracer?", ammo.Tracer == true ? "Yes" : "No", true);
            msgEmbed.AddField("Caliber", ammo.Caliber.ToString(), true);
            msgEmbed.AddField("Market Price (per round)", ammo.Avg24hPriceFormatted);
            msgEmbed.AddAlternativeItemsFooter(ammunitions, ammo);

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }

        [Command("caliber")]
        [Description("Get information about a caliber. Example usage: \"!caliber 9x19\"")]
        public async Task GetCaliberInfo(CommandContext ctx, [Description("The name of the caliber")] params string[] caliber)
        {
            string caliberString = caliber.ToStringWithSpaces();
            List<Ammunition> ammunitions = await _tarkovLensService.GetAmmunitions(caliber: caliberString);

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

            var comparisonList = new List<CaliberComparisonItem>();
            foreach (var ammo in ammunitions)
            {
                comparisonList.Add(new CaliberComparisonItem(ammo.ShortName, ammo.Caliber, ammo.Damage, ammo.Penetration, ammo.ArmorDamage, ammo.VelocityFormatted, ammo.Tracer, ammo.Avg24hPriceFormatted));
            }
            comparisonList = comparisonList.OrderBy(x => x.Name).ToList();

            string output = Formatter.Format(comparisonList);
            output = "```" + output + "```";

            await ctx.Channel.SendMessageAsync("**Resize Discord if the table does not display properly**").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
            return;
        }

        [Command("armor")]
        [Description("Get information about an armor. Example usage: \"!armor zhuk-6a\"")]
        public async Task GetArmorInfo(CommandContext ctx, [Description("The name of the armor")] params string[] name)
        {
            string nameString = name.ToStringWithSpaces().ToLower();
            List<Armor> armors = await _tarkovLensService.GetItemsByKind<Armor>(Enums.KindOfItem.Armor, nameString);
            var armor = armors.SearchForItem(nameString);

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
                Color = DiscordColor.Teal
            };
            msgEmbed.WithThumbnail(armor.Img);

            msgEmbed.AddField("Class", armor.ArmorProperties.Class.ToString(), true);
            msgEmbed.AddField("Durability", armor.ArmorProperties.Durability.ToString(), true);
            msgEmbed.AddField("Weight", $"{armor.Weight} kg", true);
            msgEmbed.AddField("Ergonomics", $"{armor.Penalties.Ergonomics}%", true);
            msgEmbed.AddField("Turn Speed", $"{armor.Penalties.Mouse}%", true);
            msgEmbed.AddField("Movement Speed", $"{armor.Penalties.Speed}%", true);

            var armorZonesString = string.Empty;
            foreach (var zone in armor.ArmorProperties.Zones)
            {
                armorZonesString += $"• {zone.FirstLetterToUpper()}{Environment.NewLine}";
            }
            msgEmbed.AddField("Protects", armorZonesString, true);

            msgEmbed.AddField("Material", armor.ArmorProperties.Material.Name.ToString().FirstLetterToUpper(), true);
            msgEmbed.AddField("Market Price", armor.Avg24hPriceFormatted);

            msgEmbed.AddAlternativeItemsFooter(armors, armor);

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }

        [Command("medical")]
        [Aliases("med")]
        [Description("Get information about a medical item or stimulant. Example usage: \"!medical salewa\"")]
        public async Task GetMedicalInfo(CommandContext ctx, [Description("The name of the medical item")] params string[] name)
        {
            string nameString = name.ToStringWithSpaces().ToLower();
            List<Medical> medicals = await _tarkovLensService.GetItemsByKind<Medical>(Enums.KindOfItem.Medical, nameString);
            var medical = medicals.SearchForItem(nameString);

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
                Color = DiscordColor.Teal
            };
            msgEmbed.WithThumbnail(medical.Img);

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

            msgEmbed.AddField("Market Price", medical.Avg24hPriceFormatted);
            msgEmbed.AddAlternativeItemsFooter(medicals, medical);

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }

        [Command("key")]
        [Aliases("k")]
        [Description("Gets information about a key. Example usage: \"!key factory\"")]
        public async Task GetKeyInfo(CommandContext ctx, [Description("The name of a key")] params string[] name)
        {
            string nameString = name.ToStringWithSpaces().ToLower();
            var keys = await _tarkovLensService.GetItemsByKind<Key>(Enums.KindOfItem.Key, nameString);
            Key key = keys.SearchForItem(nameString);

            if (key.IsNull())
            {
                var errEmbed = new DiscordEmbedBuilder
                {
                    Title = "Key not found",
                    Description = nameString,
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: errEmbed).ConfigureAwait(false);
                return;
            }

            var msgEmbed = new DiscordEmbedBuilder
            {
                Title = key.Name,
                Color = DiscordColor.Teal
            };
            msgEmbed.WithThumbnail(key.Img);

            var mapsString = key.Maps.ToBulletPointString();
            msgEmbed.AddField("Map(s)", mapsString.IsNotNullOrEmpty() ? mapsString : "-", true);
            
            msgEmbed.AddField("Rarity", key.Rarity.ToString().Humanize(LetterCasing.Sentence), true);
            msgEmbed.AddField("Market Price", key.Avg24hPriceFormatted, true);

            var usageString = key.Usage.ToBulletPointString();
            msgEmbed.AddField("Usage", usageString.IsNotNullOrEmpty() ? usageString : "-");
            
            msgEmbed.AddAlternativeItemsFooter(keys, key);

            await ctx.Channel.SendMessageAsync(embed: msgEmbed).ConfigureAwait(false);
            return;
        }
    }
}
