using ETL.Domain.Config;
using ETL.Domain.Json;
using ETLConfig.API.Models.Domain;
using ETLConfig.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ETLConfig.API.Services;

public class ConfigProcessingService : IConfigProcessingService
{
    private readonly IConfigRepository _fileService;

    public ConfigProcessingService(IConfigRepository fileService)
    {
        _fileService = fileService;
    }

    public async Task<ConfigFile> ProcessSingleConfigAsync(JsonElement json)
    {
        var config = JsonSerializer.Deserialize<ConfigFile>(json, JsonOptionsFactory.Default)
                     ?? throw new JsonException("Invalid structure.");

        ValidateConfig(config);

        await _fileService.CreateAsync(new RawConfigFile
        {
            Id = config.Id,
            JsonContent = json.GetRawText()
        });

        return config;
    }

    public async Task<List<string>> ProcessMultipleConfigsAsync(JsonElement jsonArray)
    {
        var results = new List<string>();

        foreach (var item in jsonArray.EnumerateArray())
        {
            var config = await ProcessSingleConfigAsync(item);
            results.Add(config.Id);
        }

        return results;
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

    private void ValidateConfig(ConfigFile config)
    {
        ValidateObject(config);
        ValidateObject(config.ExtractConfig);
        ValidateObject(config.ExtractConfig?.SourceInfo!);
        ValidateObject(config.TransformConfig);
        ValidateObject(config.LoadTargetConfig);
        ValidateObject(config.LoadTargetConfig?.TargetInfo!);
    }

    private static void ValidateObject(object? obj)
    {
        if (obj == null)
            throw new ValidationException("Missing or null configuration section.");

        var context = new ValidationContext(obj);
        Validator.ValidateObject(obj, context, validateAllProperties: true);
    }

}
