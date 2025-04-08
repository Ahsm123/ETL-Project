using ETL.Domain.Rules;
using ETL.Domain.Targets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class MySQLSourceInfo : SourceInfoBase
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("Table")]
    public string Table { get; set; }
    [JsonPropertyName("Columns")]
    public List<string> Columns { get; set; } = new();

    [JsonPropertyName("FilterRules")]
    public List<FilterRule> FilterRules { get; set; } = new();
    
    
   
}
