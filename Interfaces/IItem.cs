using System;
using System.Collections.Generic;
using System.Text;
using TarkovLensBot.Enums;

namespace TarkovLensBot.Interfaces
{
    public interface IItem
    {
        #region Properties
        public string Id { get; set; }
        public string BlightbusterIcon { get; }
        public string BsgId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public KindOfItem KindOfItem { get; set; }
        public float Weight { get; set; }
        public int BasePrice { get; set; }
        public int MaxStack { get; set; }
        public int LastLowestMarketPrice { get; set; }
        public int Avg24hPrice { get; set; }
        public int Avg7daysPrice { get; set; }
        public DateTime Updated { get; set; }
        public double Diff24h { get; set; }
        public double Diff7days { get; set; }
        public string Icon { get; set; }
        public string WikiLink { get; set; }
        public string Img { get; set; }
        public string ImgBig { get; set; }

        #endregion
    }
}
