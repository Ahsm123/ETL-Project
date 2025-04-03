using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.ApiTargets;

public class ApiTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("Url")]
    public string Url { get; set; }

    [JsonPropertyName("Headers")]
    public Dictionary<string, string> Headers { get; set; }

    protected ApiTargetInfoBase() { }
}
