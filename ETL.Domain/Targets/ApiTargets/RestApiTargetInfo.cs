using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.ApiTargets;

public class RestApiTargetInfo : ApiTargetInfoBase
{
    [JsonPropertyName("Method")]
    public string Method { get; set; } = "POST";

    public RestApiTargetInfo() { }
}
