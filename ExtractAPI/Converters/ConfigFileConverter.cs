using ETL.Domain.Model.SourceInfo;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExtractAPI.Converters
{

    // Json conveerter der automatisk finder ud af, hvilken type sourceInfoBase der skal bruges.
    // baseret på "SourceType" property i json
    using ETL.Domain.Model;
    using ETL.Domain.Model.SourceInfo;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ConfigFileConverter : JsonConverter<ConfigFile>
    {
        public override ConfigFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Parser hele JSON-objektet, så vi kan læse det.
            var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Henter og validerer SourceType
            if (!root.TryGetProperty("SourceType", out var sourceTypeElement))
                throw new JsonException("Missing 'SourceType'");

            var sourceType = sourceTypeElement.GetString();
            if (string.IsNullOrWhiteSpace(sourceType))
                throw new JsonException("'SourceType' cannot be null or empty");

            // Henter og deserilizer SourceInfo til den korrekte type baseret på SourceType
            if (!root.TryGetProperty("SourceInfo", out var sourceInfoElement))
                throw new JsonException("Missing 'SourceInfo'");

            var sourceInfo = sourceType!.ToLower() switch
            {
                "api" => (SourceInfoBase)JsonSerializer.Deserialize<ApiSourceBaseInfo>(sourceInfoElement.GetRawText(), options)!,
                "db" => (SourceInfoBase)JsonSerializer.Deserialize<DbSourceBaseInfo>(sourceInfoElement.GetRawText(), options)!,
                "file" => (SourceInfoBase)JsonSerializer.Deserialize<FileSourceBaseInfo>(sourceInfoElement.GetRawText(), options)!,
                _ => throw new JsonException($"Unknown source type: {sourceType}")
            };


            // Henter og validerer Id
            var id = root.GetProperty("Id").GetString();
            if (string.IsNullOrWhiteSpace(id))
                throw new JsonException("Missing 'Id'");

            // Mapper Json til ConfigFile objekt
            return new ConfigFile
            {
                Id = id,
                SourceType = sourceType,
                SourceInfo = sourceInfo!,
                Extract = JsonSerializer.Deserialize<ExtractSettings>(root.GetProperty("Extract").GetRawText(), options)!,
                Transform = JsonSerializer.Deserialize<TransformSettings>(root.GetProperty("Transform").GetRawText(), options)!,
                Load = JsonSerializer.Deserialize<LoadSettings>(root.GetProperty("Load").GetRawText(), options)!
            };
        }


        public override void Write(Utf8JsonWriter writer, ConfigFile value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

}
