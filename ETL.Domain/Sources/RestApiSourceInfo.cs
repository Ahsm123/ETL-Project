using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class RestApiSourceInfo : ApiSourceBaseInfo
{
    [JsonPropertyName("Method")]
    public string Method { get; set; } = "GET";
}