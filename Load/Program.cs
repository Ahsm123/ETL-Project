using Confluent.Kafka;
using ETL.Domain.JsonHelpers;
using ETL.Domain.SQLQueryBuilder;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Load.Messaging.Interfaces;
using Load.Messaging.Kafka.KafkaConfig;
using Load.Services;
using Load.Services.DatabaseValidation;
using Load.Services.Interfaces;
using Load.TargetWriters.Interfaces;
using Load.Workers;
using Load.Writers.DatabaseInsertingLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    var config = context.Configuration;

    // Bind Kafka settings from appsettings
    services.Configure<KafkaSettings>(config.GetSection("Kafka"));

    // Register Kafka producer using injected settings
    services.AddSingleton<IProducer<string, string>>(sp =>
    {
        var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            EnableIdempotence = kafkaSettings.Producer.EnableIdempotence,
            Acks = Enum.TryParse<Acks>(kafkaSettings.Producer.Acks, true, out var acks) ? acks : Acks.All
        };

        return new ProducerBuilder<string, string>(producerConfig).Build();
    });

    // Register SQL query builders
    services.AddSingleton<IMsSqlQueryBuilder, MsSqlQueryBuilder>();
    services.AddSingleton<IMsSqlExecutor, MsSqlExecutor>();
    services.AddSingleton<IMySqlQueryBuilder, MySQLQueryBuilder>();
    services.AddSingleton<IMySqlExecutor, MySqlExecutor>();

    // Register all ITargetWriter implementations
    var writerTypes = typeof(ITargetWriter).Assembly
        .GetTypes()
        .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

    foreach (var type in writerTypes)
    {
        services.AddSingleton(typeof(ITargetWriter), sp => ActivatorUtilities.CreateInstance(sp, type));
    }

    // Register services
    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();
    services.AddSingleton<IMessageListener, KafkaConsumer>();
    services.AddSingleton<IMessagePublisher, KafkaProducer>();
    services.AddSingleton<ILoadHandler, LoadHandler>();
    services.AddHostedService<LoadWorker>();
    services.AddSingleton<IJsonService, JsonService>();
    services.AddSingleton<IDataBaseMetadataService, HttpDatabaseMetadataService>();
    services.AddSingleton<ITableDependencySorter, TableDependencySorter>();
    services.AddHttpClient();
});

await builder.Build().RunAsync();
