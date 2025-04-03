using ETL.Domain.Events;

namespace ExtractAPI.ExtractedEvents;

public class DataExtractedEvent
{
    public ExtractedEvent Payload { get; }

    public DataExtractedEvent(ExtractedEvent payload)
    {
        Payload = payload;
    }
}
