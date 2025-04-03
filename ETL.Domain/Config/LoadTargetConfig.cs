using ETL.Domain.Targets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class LoadTargetConfig
{
    [JsonPropertyName("targetType")]
    public string TargetType { get; set; }

    [JsonIgnore]
    public TargetInfoBase TargetInfo { get; set; }

    [JsonPropertyName("targetInfo")]
    public object TargetInfoJson => TargetInfo;
}


