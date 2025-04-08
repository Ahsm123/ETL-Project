using ETL.Domain.Config;

namespace ExtractAPI.Interfaces;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(string id);
}
