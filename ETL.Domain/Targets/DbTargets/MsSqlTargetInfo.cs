using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.DbTargets;

public class MsSqlTargetInfo : DbTargetInfoBase
{
    [JsonPropertyName("UseBulkInsert")]
    public bool UseBulkInsert { get; set; }
    [JsonPropertyName("LoadMode")]
    public string LoadMode { get; set; }
    [JsonPropertyName("TargetTables")]
    public List<TargetTableConfig> TargetTables { get; set; } = new();

    public MsSqlTargetInfo() { }
}
