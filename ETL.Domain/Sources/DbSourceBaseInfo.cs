using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public abstract class DbSourceBaseInfo : SourceInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("TargetTable")]
    public string TargetTable { get; set; }
    
    
}