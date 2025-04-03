using ETL.Domain.Rules;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class ExtractConfig
{
    [JsonPropertyName("Fields")]
    public List<string> Fields { get; set; }

    [JsonPropertyName("Filters")]
    public List<FilterRule> Filters { get; set; }
}

