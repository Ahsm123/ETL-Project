using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources.Db;

public class JoinConfig
{
    [JsonPropertyName("BaseTable")]
    public string BaseTable { get; set; }

    [JsonPropertyName("Joins")]
    public List<JoinRelation> Joins { get; set; } = new();

    [JsonPropertyName("Fields")]
    public List<string> Fields { get; set; } = new();

    [JsonPropertyName("Filters")]
    public List<FilterRule>? Filters { get; set; }
}
