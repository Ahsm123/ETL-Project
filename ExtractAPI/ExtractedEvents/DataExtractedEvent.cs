using ETL.Domain.Events;

namespace ExtractAPI.ExtractedEvents;

public class DataExtractedEvent
{
    public ExtractedEvent ExtractedEvent { get; }

    public DataExtractedEvent(ExtractedEvent extractedEvent)
    {
        ExtractedEvent = extractedEvent;
    }
}
