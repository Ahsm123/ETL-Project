using ExtractAPI.Models;
using System.Text.Json;

namespace ExtractAPI.Services
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

        public async Task<ConfigFile?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get config for ID {id}, Status: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var config = JsonSerializer.Deserialize<ConfigFile>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config != null)
            {
                Console.WriteLine($"Id: {config.Id}");
                Console.WriteLine($"SourceType: {config.SourceType}");
                Console.WriteLine($"SourceInfo: {config.SourceInfo}");
                Console.WriteLine($"JsonContent: {config.JsonContent}");
            }
            else
            {
                Console.WriteLine("Could not deserialize config.");
            }

            return config;
        }
    }
}
