using ETL.Domain.Targets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;

public class LoadTargetConfig
{
    [JsonPropertyName("TargetInfo")]
    public TargetInfoBase TargetInfo { get; set; }



}


