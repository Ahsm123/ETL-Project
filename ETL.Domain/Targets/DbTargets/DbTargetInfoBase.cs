using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }
    [JsonPropertyName("TargetTable")]
    public string TargetTable { get; set; }

    [JsonPropertyName("TargetMappings")]
    public List<LoadFieldMapRule> TargetMappings { get; set; } = new();

    protected DbTargetInfoBase()
    {
    }
}
