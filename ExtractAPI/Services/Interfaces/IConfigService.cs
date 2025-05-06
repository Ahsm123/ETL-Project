using ETL.Domain.Config;

namespace ExtractAPI.Services.Interfaces;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(string id);
}
