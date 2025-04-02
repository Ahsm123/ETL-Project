using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class ExtractConfig
{
    [JsonPropertyName("fields")]
    public List<string> Fields { get; set; }

    [JsonPropertyName("filters")]
    public List<FilterRule> Filters { get; set; }
}

