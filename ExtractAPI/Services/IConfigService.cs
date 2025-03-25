using ExtractAPI.Models;

namespace ExtractAPI.Services;

public interface IConfigService
{
    Task<ConfigFile?> GetByIdAsync(string id);
}
