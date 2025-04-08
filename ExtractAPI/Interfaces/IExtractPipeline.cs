using ETL.Domain.Events;

namespace ExtractAPI.Interfaces;
public interface IExtractPipeline
{
    Task<ExtractResultEvent> RunPipelineAsync(string configId);
}
