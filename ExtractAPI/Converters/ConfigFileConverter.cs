using System.Text.Json;
using System.Text.Json.Serialization;
using ETL.Domain.Model;
using ETL.Domain.Model.SourceInfo;
using ETL.Domain.Utilities;

namespace ExtractAPI.Converters;

public class ConfigFileConverter : JsonConverter<ConfigFile>
{
    public override ConfigFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        string sourceType = root.GetProperty("SourceType").GetString()!.ToLowerInvariant();
        string id = root.GetProperty("Id").GetString()!;

        var sourceInfo = DeserializeSourceInfo(root.GetProperty("SourceInfo"), sourceType, options);

        var extractSettings = JsonSerializer.Deserialize<ExtractSettings>(root.GetProperty("Extract"), options)!;
        var transformSettings = JsonSerializer.Deserialize<TransformSettings>(root.GetProperty("Transform"), options)!;
        var loadSettings = JsonSerializer.Deserialize<LoadSettings>(root.GetProperty("Load"), options)!;

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
        var type = SourceInfoTypeMapper.GetSourceInfoType(sourceType);
        if (type == null)
            throw new JsonException($"Ukendt SourceType: {sourceType}");

        return (SourceInfoBase)JsonSerializer.Deserialize(element.GetRawText(), type, options)!;
    }
}
