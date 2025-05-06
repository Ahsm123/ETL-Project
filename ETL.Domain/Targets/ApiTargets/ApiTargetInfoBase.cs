using System.Text.Json.Serialization;

namespace ETL.Domain.Targets.ApiTargets;

public class ApiTargetInfoBase : TargetInfoBase
{
    [JsonPropertyName("Url")]
    public required string Url { get; set; }

    [JsonPropertyName("Headers")]
    public required Dictionary<string, string> Headers { get; set; }

    protected ApiTargetInfoBase() { }
}
