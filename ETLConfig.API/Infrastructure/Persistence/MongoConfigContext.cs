using ETLConfig.API.Models.Domain;
using MongoDB.Driver;

namespace ETLConfig.API.Infrastructure.Persistence;

public class MongoConfigContext
{
    public IMongoDatabase Database { get; }
    public IMongoCollection<RawConfigFile> ConfigCollection { get; }

    public MongoConfigContext(string connectionString, string dbName, string collectionName)
    {
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(dbName);
        ConfigCollection = Database.GetCollection<RawConfigFile>(collectionName);
    }
}
