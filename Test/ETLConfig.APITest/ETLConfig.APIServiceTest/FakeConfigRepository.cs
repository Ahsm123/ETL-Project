using ETLConfig.API.Models.Domain;
using ETLConfig.API.Services.Interfaces;

namespace Test.ETLConfig.API;

public class FakeConfigRepository : IConfigRepository
{
    public List<RawConfigFile> Saved = new();

    public Task CreateAsync(RawConfigFile config)
    {
        Saved.Add(config);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id) => Task.CompletedTask;
    public Task<IEnumerable<RawConfigFile>> GetAllAsync() => Task.FromResult(Enumerable.Empty<RawConfigFile>());
    public Task<RawConfigFile?> GetByIdAsync(string id) => Task.FromResult<RawConfigFile?>(null);
    public Task UpdateAsync(string id, RawConfigFile config) => Task.CompletedTask;
}
