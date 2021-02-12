using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Key : BaseItem
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("usage")]
        public List<string> Usage { get; set; }

        [JsonPropertyName("maps")]
        public List<string> Maps { get; set; }
    }
}
