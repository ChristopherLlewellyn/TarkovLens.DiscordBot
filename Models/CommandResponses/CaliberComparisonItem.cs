using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TarkovLensBot.Models.CommandResponses
{
    public class CaliberComparisonItem
    {
        public string Caliber { get; set; }
        public string Name { get; set; }
        public string Damage { get; set; }
        public string Penetration { get; set; }
        public string ArmorDamage { get; set; }
        public string Velocity { get; set; }
        public string Tracer { get; set; }
        public string MarketPrice { get; set; }

        public CaliberComparisonItem(string name, string caliber, float damage, float penetration, float armorDamage, string velocityFormatted, bool tracer, string marketPriceFormatted)
        {
            Name = name;
            Damage = damage.ToString();
            Penetration = penetration.ToString();
            ArmorDamage = armorDamage.ToString();
            Velocity = velocityFormatted;
            Tracer = tracer == true ? "Yes" : "No";
            Caliber = caliber;
            MarketPrice = marketPriceFormatted;
        }
    }
}
