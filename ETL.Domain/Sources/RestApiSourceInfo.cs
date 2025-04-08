using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

public class RestApiSourceInfo : SourceInfoBase
{
    [JsonPropertyName("Method")]
    public string Method { get; set; } = "GET";
}