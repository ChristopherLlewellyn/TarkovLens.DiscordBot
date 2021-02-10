using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TarkovLensBot.Models.Items;
using TarkovLensBot.Enums;
using TarkovLensBot.Interfaces;

namespace TarkovLensBot.Models.Items
{
    public class BaseItem : IItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// This may not actually link to an image. Certain items (ones that can be modified, e.g. Altyn) will not have an image.
        /// </summary>
        [JsonPropertyName("blightbusterIcon")]
        public string BlightbusterIcon => $"https://raw.githubusercontent.com/Blightbuster/EfTIcons/master/uid/{BsgId}.png";

        #region Tarkov-Database fields shared between all items

        [JsonPropertyName("bsgId")]
        public string BsgId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("_kind")]
        public KindOfItem KindOfItem { get; set; }

        [JsonPropertyName("weight")]
        public float Weight { get; set; }

        [JsonPropertyName("price")]
        public int BasePrice { get; set; }

        [JsonPropertyName("maxStack")]
        public int MaxStack { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("grid")]
        public Grid Grid { get; set; }
        #endregion

        #region Tarkov-Market fields
        [JsonPropertyName("lastLowestMarketPrice")]
        public int LastLowestMarketPrice { get; set; }

        [JsonPropertyName("avg24hPrice")]
        public int Avg24hPrice { get; set; }

        [JsonPropertyName("avg7daysPrice")]
        public int Avg7daysPrice { get; set; }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; }

        [JsonPropertyName("diff24h")]
        public double Diff24h { get; set; }

        [JsonPropertyName("diff7days")]
        public double Diff7days { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("wikiLink")]
        public string WikiLink { get; set; }

        [JsonPropertyName("img")]
        public string Img { get; set; }

        [JsonPropertyName("imgBig")]
        public string ImgBig { get; set; }
        #endregion
    }
}
