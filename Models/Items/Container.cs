﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Enums;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Models.Items
{
    public class Container : BaseItem
    {
        public List<StorageGrid> Grids { get; set; }
    }
}
