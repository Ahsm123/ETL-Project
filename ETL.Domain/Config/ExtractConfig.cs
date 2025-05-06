using ETL.Domain.Rules;
using ETL.Domain.Sources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ETL.Domain.Model;

public class ExtractConfig
{
    [Required]
    [JsonPropertyName("SourceInfo")]
    public SourceInfoBase SourceInfo { get; set; }

    [Required]
    [JsonPropertyName("Fields")]
    public List<string> Fields { get; set; }

    [JsonPropertyName("Filters")]
    public List<FilterRule>? Filters { get; set; }
}

