using ETL.Domain.Model;
using ETL.Domain.Sources;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class ConfigFile
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; }

    [JsonPropertyName("sourceInfo")]
    public SourceInfoBase SourceInfo { get; set; }

    [JsonPropertyName("extractConfig")]
    public ExtractConfig Extract { get; set; }

    [JsonPropertyName("transformConfig")]
    public TransformConfig Transform { get; set; }

    [JsonPropertyName("loadTargetConfig")]
    public LoadTargetConfig Load { get; set; }
}
