using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Config;
public class LoadTargetConfig
{
    [JsonPropertyName("TargetInfo")]
    public TargetInfoBase TargetInfo { get; set; }
    [JsonPropertyName("Tables")]
    public List<TargetTableConfig> Tables { get; set; } = new();



}



