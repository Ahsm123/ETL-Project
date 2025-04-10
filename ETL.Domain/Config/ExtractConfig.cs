using ETL.Domain.Rules;
using ETL.Domain.Sources;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class ExtractConfig
{
    [JsonPropertyName("SourceInfo")]
    public SourceInfoBase SourceInfo { get; set; }

    [JsonPropertyName("Fields")]
    public List<string> Fields { get; set; }

    [JsonPropertyName("Filters")]
    public List<FilterRule>? Filters { get; set; }
}
//select all from "targettable" where "apply filter"

