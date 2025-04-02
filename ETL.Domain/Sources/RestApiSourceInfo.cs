using ETL.Domain.Attributes;
using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

[SourceType("api")]
public class RestApiSourceInfo : SourceInfoBase
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }

}
