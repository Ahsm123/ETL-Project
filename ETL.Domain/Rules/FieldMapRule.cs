using System.Text.Json.Serialization;

namespace ETL.Domain.Rules;
public class FieldMapRule
{
    [JsonPropertyName("SourceField")]
    public string SourceField { get; set; }

    [JsonPropertyName("TargetField")]
    public string TargetField { get; set; }
}