using ETL.Domain.Model.TargetInfo;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class LoadTargetConfig
{
    public string TargetType { get; set; }

    [JsonIgnore]
    public TargetInfoBase TargetInfo { get; set; }

    [JsonPropertyName("TargetInfo")]
    public object TargetInfoJson => TargetInfo;
}

