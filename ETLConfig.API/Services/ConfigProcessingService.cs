using ETL.Domain.Config;
using ETL.Domain.JsonHelpers;
using ETLConfig.API.Models.Domain;
using ETLConfig.API.Services.Interfaces;
using System.Text.Json;

public class ConfigProcessingService : IConfigProcessingService
{
    private readonly IConfigRepository _fileService;
    private readonly IJsonService _jsonService;
    private readonly IConfigValidator _configValidator;

    public ConfigProcessingService(
        IConfigRepository fileService,
        IJsonService jsonService,
        IConfigValidator configValidator)
    {
        _fileService = fileService;
        _jsonService = jsonService;
        _configValidator = configValidator;
    }

    public async Task<ConfigFile> ProcessSingleConfigAsync(JsonElement json)
    {
        var config = _jsonService.Deserialize<ConfigFile>(json.GetRawText())
                     ?? throw new JsonException("Invalid structure.");

        _configValidator.Validate(config); // <-- central validation call

        await _fileService.CreateAsync(new RawConfigFile
        {
            Id = config.Id,
            JsonContent = json.GetRawText()
        });

        return config;
    }


    public async Task<List<JsonElement>> GetAllConfigsAsync()
    {
        var configs = await _fileService.GetAllAsync();

        return configs
            .Select(cfg => JsonDocument.Parse(cfg.JsonContent).RootElement)
            .ToList();
    }

    public async Task<JsonElement?> GetConfigByIdAsync(string id)
    {
        var config = await _fileService.GetByIdAsync(id);
        if (config == null) return null;

        return JsonDocument.Parse(config.JsonContent).RootElement;
    }

    public async Task UpdateConfigAsync(string id, JsonElement json)
    {
        await _fileService.UpdateAsync(id, new RawConfigFile
        {
            Id = id,
            JsonContent = json.GetRawText()
        });
    }

    public async Task DeleteConfigAsync(string id)
    {
        await _fileService.DeleteAsync(id);
    }

}
