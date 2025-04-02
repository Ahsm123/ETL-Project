using ETL.Domain.Config;
using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using ETL.Domain.Model.TargetInfo;
using ETL.Domain.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transform.Converters;

public class ExtractedPayloadConverter : JsonConverter<ExtractedPayload>
{
    public override ExtractedPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var id = root.GetProperty("Id").GetString()!;
        var sourceType = root.GetProperty("SourceType").GetString()!;
        var transform = JsonSerializer.Deserialize<TransformConfig>(root.GetProperty("Transform"), options)!;
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(root.GetProperty("Data"), options)!;

        var targetType = root.GetProperty("Load").GetProperty("TargetType").GetString()!;
        var targetInfoJson = root.GetProperty("Load").GetProperty("TargetInfo").GetRawText();

        var targetInfoType = TargetTypeMapper.GetTargetInfoType(targetType.ToLowerInvariant());
        if (targetInfoType == null)
            throw new JsonException($"Unknown TargetType: {targetType}");

        var targetInfo = (TargetInfoBase)JsonSerializer.Deserialize(targetInfoJson, targetInfoType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        var loadSettings = new LoadTargetConfig
        {
            TargetType = targetType,
            TargetInfo = targetInfo
        };

        return new ExtractedPayload
        {
            Id = id,
            SourceType = sourceType,
            Transform = transform,
            Load = loadSettings,
            Data = data
        };
    }

    public override void Write(Utf8JsonWriter writer, ExtractedPayload value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}
