using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class MySQLSourceInfo : DbSourceBaseInfo
{

    [JsonPropertyName("Table")]
    public string Table { get; set; }
    [JsonPropertyName("Columns")]
    public List<string> Columns { get; set; } = new();


}
