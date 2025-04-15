namespace ExtractAPI.Messaging;

public class EventRoutingOptions
{
    public Dictionary<string, string> EventTopics { get; set; } = new();
}
