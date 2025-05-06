using ETL.Domain.Config;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;

public class TransformedEvent
{
    [JsonPropertyName("PipelineId")]
    public string PipelineId { get; set; }

    [JsonPropertyName("LoadConfig")]
    public LoadConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("Record")]
    public RawRecord Record { get; set; }
}

