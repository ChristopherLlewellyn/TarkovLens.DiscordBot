using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class ModificationMuzzle : BaseModification
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("velocity")]
        public float Velocity { get; set; }
    }
}
