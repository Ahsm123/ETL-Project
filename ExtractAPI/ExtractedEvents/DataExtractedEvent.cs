using ETL.Domain.Model.DTOs;

namespace ExtractAPI.ExtractedEvents;

public class DataExtractedEvent
{
    public ExtractedPayload Payload { get; }

    public DataExtractedEvent(ExtractedPayload payload)
    {
        Payload = payload;
    }
}
