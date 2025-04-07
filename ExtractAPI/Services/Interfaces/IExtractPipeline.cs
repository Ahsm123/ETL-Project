using ETL.Domain.Events;

namespace ExtractAPI.Services.Interfaces;
public interface IExtractPipeline
{
    Task<ExtractResultEvent> ExtractAsync(string configId);
}
