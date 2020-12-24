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
    }
}
