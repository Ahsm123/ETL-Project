using System.Text.Json.Serialization;

namespace ETL.Domain.Sources.Db;


public class JoinRelation
{
    [JsonPropertyName("JoinType")]
    public string JoinType { get; set; } // "INNER", "LEFT", etc.

    [JsonPropertyName("Table")]
    public string Table { get; set; }

    [JsonPropertyName("On")]
    public List<JoinCondition> On { get; set; } = new();
}
