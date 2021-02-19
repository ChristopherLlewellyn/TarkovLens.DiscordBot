using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TarkovLensBot.Helpers.ExtensionMethods;

namespace TarkovLensBot.Models.Other
{
    public class LocationWithMaps
    {
        public string Location { get; set; }
        public List<string> MapUrls { get; set; }
        public string MapGenieUrl { get; set; }

        public LocationWithMaps(string locationName)
        {
            List<string> locations = new List<string>()
            {
                "Customs",
                "The Lab (Labs)",
                "Interchange",
                "Factory",
                "Reserve",
                "Shoreline",
                "Woods"
            };

            locations = locations.OrderBy(x => x.Length).ToList();
            var location = locations.Where(x => x.ToLower() == locationName.ToLower()).FirstOrDefault();

            if (location.IsNull())
                location = locations.Where(x => x.ToLower().Contains(locationName.ToLower())).FirstOrDefault();

            if (location.IsNotNullOrEmpty())
            {
                Location = location;

                switch (location)
                {
                    case "Customs":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/5/55/CustomsLargeExpansionGloryMonki.png/revision/latest?cb=20200805222908",
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/2/27/2021_Customs_by_PaulRIISK.png/revision/latest?cb=20210202221431"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/customs";
                        break;

                    case "The Lab (Labs)":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/0/06/TheLabBasementByLogiwonk.png/revision/latest?cb=20190210195439",
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/3/35/TheLabFirstFloorByLogiwonk.png/revision/latest?cb=20190210195441",
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/0/0f/TheLabSecondFloorByLogiwonk.png/revision/latest?cb=20190210195443"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/lab";
                        break;

                    case "Interchange":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/e/e5/InterchangeMap_Updated_4.24.2020.png/revision/latest?cb=20200424115934"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/interchange";
                        break;

                    case "Factory":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/c/cd/Factory_3D_b_Johnny_Tushonka.jpg/revision/latest?cb=20200130011910"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/factory";
                        break;

                    case "Reserve":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/4/42/3D_Map_by_loweffortsaltbox.png/revision/latest?cb=20200410160036"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/reserve"; // Requires pro version of MapGenie to open
                        break;

                    case "Shoreline":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/a/ac/Shoreline_Map_-_12.7.png/revision/latest?cb=20200809112827",
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/e/e1/Actual_caches_37_map_shoreline.jpg/revision/latest?cb=20200105023458"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/shoreline";
                        break;

                    case "Woods":
                        MapUrls = new List<string>()
                        {
                            "https://static.wikia.nocookie.net/escapefromtarkov_gamepedia/images/0/05/Glory4lyfeWoods_map_v4_marked.png/revision/latest?cb=20210122154225"
                        };
                        MapGenieUrl = "https://mapgenie.io/tarkov/maps/woods";
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
