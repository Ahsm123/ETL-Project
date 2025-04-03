using ETL.Domain.Targets.ApiTargets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class RestApiSourceInfo : SourceInfoBase
{
    [JsonPropertyName("Url")]
    public string Url { get; set; }

    [JsonPropertyName("Headers")]
    public Dictionary<string, string> Headers { get; set; }
}