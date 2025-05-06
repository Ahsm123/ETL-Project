using ETL.Domain.Sources.Db;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class MsSqlSourceInfo : DbSourceBaseInfo
{
    [JsonPropertyName("UseTrustedConnection")]
    public bool UseTrustedConnection { get; set; } = false;

    [JsonPropertyName("JoinConfig")]
    public JoinConfig? JoinConfig { get; set; } // Optional - used for advanced queries
}