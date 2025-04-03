using ETL.Domain.Config;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;


public class TransformedEvent
{
    [JsonPropertyName("PipelineId")]
    public string PipelineId { get; set; }

    [JsonPropertyName("LoadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("Data")]
    public Dictionary<string, object> Data { get; set; }
}
