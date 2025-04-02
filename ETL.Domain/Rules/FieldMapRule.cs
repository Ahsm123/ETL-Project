using System.Text.Json.Serialization;

namespace ETL.Domain.Rules;

public class FieldMapRule
{
    [JsonPropertyName("sourceField")]
    public string SourceField { get; set; }

    [JsonPropertyName("targetField")]
    public string TargetField { get; set; }
}
