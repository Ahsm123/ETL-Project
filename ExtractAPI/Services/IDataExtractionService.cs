using ETL.Domain.Events;

namespace ExtractAPI.Services;
public interface IDataExtractionService
{
    Task<ExtractResultEvent> ExtractAsync(string configId);
}
