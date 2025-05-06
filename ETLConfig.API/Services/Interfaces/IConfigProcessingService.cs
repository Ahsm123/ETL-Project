using ETL.Domain.Config;
using System.Text.Json;

namespace ETLConfig.API.Services.Interfaces;

public interface IConfigProcessingService
{
    Task<ConfigFile> ProcessSingleConfigAsync(JsonElement json);

    Task<List<JsonElement>> GetAllConfigsAsync();
    Task<JsonElement?> GetConfigByIdAsync(string id);
    Task UpdateConfigAsync(string id, JsonElement json);
    Task DeleteConfigAsync(string id);
}

