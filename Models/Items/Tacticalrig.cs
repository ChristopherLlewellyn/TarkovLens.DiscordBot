using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Tacticalrig : BaseItem
    {
        [JsonPropertyName("grids")]
        public List<StorageGrid> Grids { get; set; }

        [JsonPropertyName("penalties")]
        public Penalties Penalties { get; set; }
    }
}
