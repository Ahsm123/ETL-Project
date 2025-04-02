using ETL.Domain.Config;
using ETL.Domain.Model;
using ETL.Domain.Sources;
using ETL.Domain.Targets;
using ETL.Domain.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExtractAPI.Converters;

public class ConfigFileConverter : JsonConverter<ConfigFile>
{
    public override ConfigFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        string sourceType = root.GetProperty("SourceType").GetString()!.ToLowerInvariant();
        string id = root.GetProperty("Id").GetString()!;
        string targetType = root.GetProperty("LoadTargetConfig").GetProperty("TargetType").GetString()!.ToLowerInvariant();

        var sourceInfo = DeserializeSourceInfo(root.GetProperty("SourceInfo"), sourceType, options);
        var extractSettings = JsonSerializer.Deserialize<ExtractConfig>(root.GetProperty("ExtractConfig"), options)!;
        var transformSettings = JsonSerializer.Deserialize<TransformConfig>(root.GetProperty("TransformConfig"), options)!;

        var targetInfoType = TargetTypeMapper.GetTargetInfoType(targetType);
        if (targetInfoType == null)
            throw new JsonException($"Unknown TargetType: {targetType}");

        var targetInfo = (TargetInfoBase)JsonSerializer.Deserialize(
            root.GetProperty("LoadTargetConfig").GetProperty("TargetInfo").GetRawText(),
            targetInfoType,
            options
        )!;

        var loadSettings = new LoadTargetConfig
        {
            TargetType = targetType,
            TargetInfo = targetInfo
        };

        return new ConfigFile
        {
            Id = id,
            SourceType = sourceType,
            SourceInfo = sourceInfo,
            Extract = extractSettings,
            Transform = transformSettings,
            Load = loadSettings
        };
    }

    public override void Write(Utf8JsonWriter writer, ConfigFile value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    private static SourceInfoBase DeserializeSourceInfo(JsonElement element, string sourceType, JsonSerializerOptions options)
    {
        var type = SourceTypeMapper.GetSourceInfoType(sourceType);
        if (type == null)
            throw new JsonException($"Ukendt SourceType: {sourceType}");

        return (SourceInfoBase)JsonSerializer.Deserialize(element.GetRawText(), type, options)!;
    }
}
