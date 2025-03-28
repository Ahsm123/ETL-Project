using ETL.Domain.Model;
using System.Text.Json;

namespace ExtractAPI.Services;

public interface IDataExtractionService
{
    Task<ConfigFile> ExtractAndDispatchAsync(string configId);
}
