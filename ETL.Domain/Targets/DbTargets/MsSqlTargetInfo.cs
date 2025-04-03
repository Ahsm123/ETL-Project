using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public class MsSqlTargetInfo : DbTargetInfoBase
{
    [JsonPropertyName("UseBulkInsert")]
    public bool UseBulkInsert { get; set; }

    public MsSqlTargetInfo() { }
}
