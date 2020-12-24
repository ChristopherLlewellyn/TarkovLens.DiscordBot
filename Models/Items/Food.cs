using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Enums;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Food : BaseItem
    {
        [JsonPropertyName("type")]
        public FoodType Type { get; set; }

        [JsonPropertyName("resources")]
        public int Resources { get; set; }

        [JsonPropertyName("useTime")]
        public int UseTime { get; set; }

        [JsonPropertyName("effects")]
        public Effects Effects { get; set; }
    }
}
