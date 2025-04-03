using ETL.Domain.Attributes;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

[TargetType("mssql")]
public class MsSqlTargetInfo : DbTargetInfoBase
{
    [JsonPropertyName("useBulkInsert")]
    public bool UseBulkInsert { get; set; }

    public MsSqlTargetInfo()
    {
    }
}
