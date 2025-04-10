using ETLConfig.API.Infrastructure.Persistence;
using ETLConfig.API.Models.Domain;
using ETLConfig.API.Services.Interfaces;
using MongoDB.Driver;

namespace ETLConfig.API.Services
{
    public class MongoConfigRepository : IConfigRepository
    {
        private readonly IMongoCollection<RawConfigFile> _collection;

        public MongoConfigRepository(MongoConfigContext context)
        {
            _collection = context.ConfigCollection;
        }

        public async Task CreateAsync(RawConfigFile config)
        {
            await _collection.InsertOneAsync(config);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }

        public async Task<RawConfigFile?> GetByIdAsync(string id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RawConfigFile>> GetAllAsync()
        {
            return await _collection.Find(FilterDefinition<RawConfigFile>.Empty).ToListAsync();
        }

        public async Task UpdateAsync(string id, RawConfigFile config)
        {
            await _collection.ReplaceOneAsync(c => c.Id == id, config);
        }
    }
}
