using System.Text.Json.Serialization;

namespace ETL.Domain.Rules;

public class FilterRule
{
    [JsonPropertyName("Field")]
    public string Field { get; set; }

    [JsonPropertyName("Operator")]
    public string Operator { get; set; }

    [JsonPropertyName("Value")]
    public string Value { get; set; }
}
