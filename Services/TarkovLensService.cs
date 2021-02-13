using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TarkovLensBot.Enums;
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

        /// <summary>
        /// Gets a list of items that match the provided item name. Returns an empty list if no items were found.
        /// </summary>
        /// <param name="nameOfItem"></param>
        /// <returns></returns>
        public async Task<List<BaseItem>> GetItemsBySearch(string nameOfItem)
        {
            var items = new List<BaseItem>();
            var response = await httpClient.GetAsync($"item/search?name={nameOfItem}");
            
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                items = JsonSerializer.Deserialize<List<BaseItem>>(json, options);

            }

            return items;
        }

        /// <summary>
        /// Get a list of all ammunitions matching the parameters. Returns an empty list if no item was found.
        /// </summary>
        /// <param name="nameOfItem">OPTIONAL: search for ammunitions by name</param>
        /// /// <param name="caliber">OPTIONAL: search for ammunitions by caliber</param>
        /// <returns>A list of Ammunition objects.</returns>
        public async Task<List<Ammunition>> GetAmmunitions(string nameOfItem = null, string caliber = null)
        {
            var items = new List<Ammunition>();
            var response = await httpClient.GetAsync($"item/kind/{KindOfItem.Ammunition}?name={nameOfItem}&caliber={caliber}");
            
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                items = JsonSerializer.Deserialize<List<Ammunition>>(json, options);
            }

            return items;
        }

        /// <summary>
        /// Gets a list of items that match the provided parameters. Returns an empty list if no item was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind"></param>
        /// <param name="nameOfItem"></param>
        /// <returns></returns>
        public async Task<List<T>> GetItemsByKind<T>(KindOfItem kind, string nameOfItem = null) where T : IItem
        {
            var items = new List<T>();
            var response = await httpClient.GetAsync($"item/kind/{kind}?name={nameOfItem}");
            
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                items = JsonSerializer.Deserialize<List<T>>(json, options);
            }

            return items;
        }
    }
}
