using ETL.Domain.Config;
using ETL.Domain.Model;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;

public class ExtractedEvent
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("TransformConfig")]
    public TransformConfig TransformConfig { get; set; }

    [JsonPropertyName("LoadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("Data")]
    public Dictionary<string, object> Data { get; set; }
}