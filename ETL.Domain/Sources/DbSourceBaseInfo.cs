using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class DbSourceBaseInfo : SourceInfoBase
{
    [JsonPropertyName("connectionString")]
    public string ConnectionString { get; set; }
    [JsonPropertyName("targetTable")]
    public string TargetTable { get; set; }
    [JsonPropertyName("query")]
    public string Query { get; set; }
    [JsonPropertyName("provider")]
    public string Provider { get; set; }

}
