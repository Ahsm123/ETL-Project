using ETL.Domain.Model;
using System.Text.Json;

namespace ExtractAPI.Services;

public interface IExtractService
{
    Task<ConfigFile> ExtractAsync(string configId);
}
