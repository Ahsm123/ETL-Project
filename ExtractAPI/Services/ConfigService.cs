using ExtractAPI.Models;
using System.Text.Json;
;
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
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ConfigFile>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
