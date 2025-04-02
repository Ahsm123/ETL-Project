namespace ETL.Domain.Events;

public class ExtractResultEvent
{
    public string PipelineId { get; set; }
    public int MessagesSent { get; set; }
}
