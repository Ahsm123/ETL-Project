using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class TransformConfig
{
    [JsonPropertyName("Mappings")]
    public List<FieldMapRule> Mappings { get; set; }

    [JsonPropertyName("Filters")]
    public List<FilterRule> Filters { get; set; }
}

