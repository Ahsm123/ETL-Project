using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("connectionString")]
    public string ConnectionString { get; set; }
    [JsonPropertyName("targetTable")]
    public string TargetTable { get; set; }

    protected DbTargetInfoBase()
    {
    }
}
