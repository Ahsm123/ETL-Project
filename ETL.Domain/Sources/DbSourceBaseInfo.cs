using ETL.Domain.Targets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class DbSourceBaseInfo : SourceInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("Query")]
    public string Query { get; set; }

    [JsonPropertyName("Provider")]
    public string Provider { get; set; }
}
