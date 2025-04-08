using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public abstract class DbSourceBase : SourceInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("Query")]
    public string Query { get; set; }
}