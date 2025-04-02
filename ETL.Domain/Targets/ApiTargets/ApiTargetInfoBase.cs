using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.ApiTargets;

public class ApiTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }

    protected ApiTargetInfoBase()
    {
    }
}
