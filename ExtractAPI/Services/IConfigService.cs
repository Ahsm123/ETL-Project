using ETL.Domain.Config;

namespace ExtractAPI.Services;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(string id);
}
