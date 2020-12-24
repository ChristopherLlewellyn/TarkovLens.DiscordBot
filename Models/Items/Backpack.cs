﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Enums;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Backpack : BaseItem
    {
        [JsonPropertyName("grids")]
        public List<BackpackGrid> Grids { get; set; }

        [JsonPropertyName("penalties")]
        public Penalties Penalties { get; set; }
    }

    public class BackpackGrid
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("maxWeight")]
        public float MaxWeight { get; set; }
    }
}

