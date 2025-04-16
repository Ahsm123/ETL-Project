using ETL.Domain.JsonHelpers;
using ETL.Domain.SQLQueryBuilder;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Load.Interfaces;
using Load.Messaging;
using Load.Services;
using Load.Workers;
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

    // Register other services
    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();
    services.AddSingleton<IMessageListener, KafkaMessageListener>();
    services.AddSingleton<ILoadHandler, LoadHandler>();
    services.AddHostedService<LoadWorker>();
    services.AddSingleton<IJsonService, JsonService>();
});


await builder.Build().RunAsync();
