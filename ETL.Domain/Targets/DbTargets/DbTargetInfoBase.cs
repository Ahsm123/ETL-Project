using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }
    [JsonPropertyName("TargetTable")]
    public string TargetTable { get; set; }

    protected DbTargetInfoBase()
    {
    }
}
