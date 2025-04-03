using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class TransformConfig
{
    [JsonPropertyName("mappings")]
    public List<FieldMapRule> Mappings { get; set; }

    [JsonPropertyName("filters")]
    public List<FilterRule> Filters { get; set; }
}

