using ETL.Domain.Model;

namespace ExtractAPI.Services;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(string id);
}
