using Extract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extract.Services
{
    public class ConfigService : IConfigService
    {
        private readonly HttpClient _httpClient;

        public ConfigService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

        }

        public async Task<ConfigFile?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ConfigFile>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
