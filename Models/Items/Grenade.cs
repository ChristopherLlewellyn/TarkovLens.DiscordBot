using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Grenade : BaseItem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("delay")]
        public float Delay { get; set; }

        [JsonPropertyName("fragCount")]
        public int FragCount { get; set; }

        [JsonPropertyName("minDistance")]
        public int MinDistance { get; set; }

        [JsonPropertyName("maxDistance")]
        public int MaxDistance { get; set; }

        [JsonPropertyName("contusionDistance")]
        public int ContusionDistance { get; set; }

        [JsonPropertyName("strength")]
        public int Strength { get; set; }

        [JsonPropertyName("emitTime")]
        public int EmitTime { get; set; }
    }
}
