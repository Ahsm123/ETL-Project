using System.Text.Json.Serialization;

namespace ETL.Domain.Rules;

public class FilterRule
{
    [JsonPropertyName("field")]
    public string Field { get; set; }

    [JsonPropertyName("operator")]
    public string Operator { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}
