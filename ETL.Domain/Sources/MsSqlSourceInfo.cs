using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class MsSqlSourceInfo : DbSourceBase
{
    [JsonPropertyName("UseTrustedConnection")]
    public bool UseTrustedConnection { get; set; } = false;
}