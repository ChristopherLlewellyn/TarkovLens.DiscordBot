using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Melee : BaseItem
    {
        [JsonPropertyName("slash")]
        public MeleeAttack Slash { get; set; }

        [JsonPropertyName("stab")]
        public MeleeAttack Stab { get; set; }
    }

    public class MeleeAttack
    {
        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        [JsonPropertyName("rate")]
        public int Rate { get; set; }

        [JsonPropertyName("range")]
        public float Range { get; set; }

        [JsonPropertyName("consumption")]
        public int Consumption { get; set; }
    }
}
