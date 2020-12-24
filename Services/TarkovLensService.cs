using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TarkovLensBot.Interfaces;
using TarkovLensBot.Models.Items;

namespace TarkovLensBot.Services
{
    public class TarkovLensService
    {
        private readonly HttpClient httpClient;

        public TarkovLensService(HttpClient client, IConfiguration Configuration)
        {
            var config = new Config();
            Configuration.Bind(config);

            client.BaseAddress = new Uri(config.TarkovLensAPIUrl);
            httpClient = client;
        }

        public async Task<List<BaseItem>> GetItemsBySearch(string nameOfItem)
        {
            var response = await httpClient.GetAsync($"item/search?name={nameOfItem}");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<BaseItem>>(json);

            return items;
        }
    }
}
