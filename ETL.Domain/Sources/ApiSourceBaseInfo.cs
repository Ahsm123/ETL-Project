using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class ApiSourceBaseInfo : SourceInfoBase
{
    [Required]
    [JsonPropertyName("Url")]
    public string Url { get; set; }

    [JsonPropertyName("Headers")]
    public Dictionary<string, string> Headers { get; set; } = new();
}