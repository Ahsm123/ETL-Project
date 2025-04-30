using ETLConfig.API.Models.Domain;
using MongoDB.Driver;

namespace ETLConfig.API.Infrastructure.Persistence;

public class MongoConfigContext
{
    public IMongoDatabase Database { get; }
    public IMongoCollection<RawConfigFile> ConfigCollection { get; }

    public MongoConfigContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        ConfigCollection = database.GetCollection<RawConfigFile>(settings.CollectionName);
    }
}
