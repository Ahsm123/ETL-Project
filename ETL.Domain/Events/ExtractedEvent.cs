using ETL.Domain.Config;
using ETL.Domain.Model;
using System.Text.Json.Serialization;

namespace ETL.Domain.Events;

public class ExtractedEvent
{
    [JsonPropertyName("PipelineId")]
    public required string PipelineId { get; set; }

    [JsonPropertyName("TransformConfig")]
    public required TransformConfig TransformConfig { get; set; }

    [JsonPropertyName("LoadTargetConfig")]
    public required LoadConfig LoadTargetConfig { get; set; }

    [JsonPropertyName("Record")]
    public RawRecord Record { get; set; }
}