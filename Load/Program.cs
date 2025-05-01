using Confluent.Kafka;
using ETL.Domain.JsonHelpers;
using ETL.Domain.SQLQueryBuilder;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Load.Interfaces;
using Load.Messaging;
using Load.Services;
using Load.Services.DatabaseValidation;
using Load.Workers;
using Load.Writers.DatabaseInsertingLogic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{

// Register dependencies for MsSqlTargetWriter
services.AddSingleton<IMsSqlQueryBuilder, MsSqlQueryBuilder>();
services.AddSingleton<IMsSqlExecutor, MsSqlExecutor>();

// Register dependencies for MySqlTargetWriter (if needed)
services.AddSingleton<IMySqlQueryBuilder, MySQLQueryBuilder>();
services.AddSingleton<IMySqlExecutor, MySqlExecutor>();

// Dynamically register all ITargetWriter implementations
var writerTypes = typeof(ITargetWriter).Assembly
    .GetTypes()
    .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) &&
                t is { IsClass: true, IsAbstract: false });

foreach (var type in writerTypes)
{
    services.AddSingleton(typeof(ITargetWriter), serviceProvider =>
        ActivatorUtilities.CreateInstance(serviceProvider, type));
}
    //register Kafka producer
    services.AddSingleton<IProducer<string, string>>(sp =>
    {
        var config = new ProducerConfig
        {
            BootstrapServers = sp.GetRequiredService<IConfiguration>()["Kafka:BootstrapServers"]!,
            EnableIdempotence = true,
            Acks = Acks.All
        };

        return new ProducerBuilder<string, string>(config).Build();
    });

    // Register other services
services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();
services.AddSingleton<IMessageListener, KafkaMessageListener>();
services.AddSingleton<ILoadHandler, LoadHandler>();
services.AddHostedService<LoadWorker>();
services.AddSingleton<IJsonService, JsonService>();
services.AddSingleton<IDataBaseMetadataService, HttpDatabaseMetadataService>();
services.AddSingleton<ITableDependencySorter, TableDependencySorter>();
services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
services.AddHttpClient();
services.AddSingleton<IDataBaseMetadataService, HttpDatabaseMetadataService>();
});


await builder.Build().RunAsync();
