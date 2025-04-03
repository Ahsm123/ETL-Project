using ETL.Domain.Attributes;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.ApiTargets;

[TargetType("restapi")]
public class RestApiTargetInfo
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = "POST";

    public RestApiTargetInfo()
    {
    }
}
