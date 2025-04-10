using ETLConfig.API.Models.Domain;

namespace ETLConfig.API.Services.Interfaces
{
    public interface IConfigRepository
    {
        Task<RawConfigFile?> GetByIdAsync(string id);
        Task<IEnumerable<RawConfigFile>> GetAllAsync();
        Task CreateAsync(RawConfigFile config);
        Task UpdateAsync(string id, RawConfigFile config);
        Task DeleteAsync(string id);
    }
}
