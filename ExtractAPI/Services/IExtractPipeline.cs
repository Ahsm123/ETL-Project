using ETL.Domain.Events;

namespace ExtractAPI.Services;
public interface IExtractPipeline
{
    Task<ExtractResultEvent> ExtractAsync(string configId);
}
