using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovLensBot
{
    public class Config
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string TarkovLensAPIUrl { get; set; }
    }
}
