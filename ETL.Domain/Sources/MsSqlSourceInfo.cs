using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class MsSqlSourceInfo : DbSourceBaseInfo
{
    [JsonPropertyName("UseTrustedConnection")]
    public bool UseTrustedConnection { get; set; } = false;
}