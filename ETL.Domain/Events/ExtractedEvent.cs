using ETL.Domain.Config;
using ETL.Domain.Model;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;

public class ExtractedEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; }

    [JsonPropertyName("transformConfig")]
    public TransformConfig TransformConfig { get; set; }

    [JsonPropertyName("loadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; }
}
