using ETL.Domain.Events;

namespace ExtractAPI.Interfaces;
public interface IExtractPipeline
{
    Task<ExtractResultEvent> ExtractAsync(string configId);
}
