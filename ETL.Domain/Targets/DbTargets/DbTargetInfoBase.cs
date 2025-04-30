using ETL.Domain.Rules;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public required string ConnectionString { get; set; }
    //[JsonPropertyName("TargetTable")]
    //public string TargetTable { get; set; }
    [JsonPropertyName("LoadMode")]
    public string LoadMode { get; set; }

    //[JsonPropertyName("TargetMappings")]
    //public List<LoadFieldMapRule> TargetMappings { get; set; } = new();
    //[JsonPropertyName("TargetTables")]
    //public List<TargetTableConfig> TargetTables { get; set; } = new();its 

    protected DbTargetInfoBase()
    {
    }
}
