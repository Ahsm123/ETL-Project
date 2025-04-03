using ETL.Domain.Config;
using ETL.Domain.Events;
using ETL.Domain.Model;
using ETL.Domain.Targets;
using ETL.Domain.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transform.Converters;

public class ExtractedEventConverter : JsonConverter<ExtractedEvent>
{
    public override ExtractedEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var id = root.GetProperty("id").GetString();
        var sourceType = root.GetProperty("sourceType").GetString();

        var transform = JsonSerializer.Deserialize<TransformConfig>(root.GetProperty("transformConfig"), options)!;
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(root.GetProperty("data"), options)!;

        var loadElement = root.GetProperty("loadTargetConfig");
        var targetType = loadElement.GetProperty("targetType").GetString()!;
        var targetInfoJson = loadElement.GetProperty("targetInfo").GetRawText();

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

        return new ExtractedEvent
        {
            Id = id!,
            SourceType = sourceType!,
            TransformConfig = transform,
            LoadTargetConfig = loadSettings,
            Data = data
        };
    }

    public override void Write(Utf8JsonWriter writer, ExtractedEvent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id);
        writer.WriteString("sourceType", value.SourceType);

        writer.WritePropertyName("transformConfig");
        JsonSerializer.Serialize(writer, value.TransformConfig, options);

        writer.WritePropertyName("loadTargetConfig");
        JsonSerializer.Serialize(writer, value.LoadTargetConfig, options);

        writer.WritePropertyName("data");
        JsonSerializer.Serialize(writer, value.Data, options);

        writer.WriteEndObject();
    }
}

