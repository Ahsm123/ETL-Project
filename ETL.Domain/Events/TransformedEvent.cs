using ETL.Domain.Config;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;

public class TransformedEvent
{
    [JsonPropertyName("pipelineId")]
    public string PipelineId { get; set; }

    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; }

    [JsonPropertyName("loadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; }
}
