using ETL.Domain.Model;
using ETL.Domain.Sources;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class ConfigFile
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("SourceInfo")]
    public SourceInfoBase SourceInfo { get; set; }

    [JsonPropertyName("ExtractConfig")]
    public ExtractConfig ExtractConfig { get; set; }

    [JsonPropertyName("TransformConfig")]
    public TransformConfig TransformConfig { get; set; }

    [JsonPropertyName("LoadTargetConfig")]
    public LoadTargetConfig LoadTargetConfig { get; set; }
}
