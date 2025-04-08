using ETL.Domain.Events;

namespace ExtractAPI.Models;

public class DataExtractedEvent
{
    public ExtractedEvent ExtractedEvent { get; }

    public DataExtractedEvent(ExtractedEvent extractedEvent)
    {
        ExtractedEvent = extractedEvent;
    }
}
