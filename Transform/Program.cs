using ETL.Domain.JsonHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Transform.Interfaces;
using Transform.Messaging;
using Transform.Services;
using Transform.Workers;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        // Infrastructure
        services.AddSingleton<IMessageListener, KafkaMessageListener>();
        services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();

        // Transformation pipeline
        services.AddSingleton<FilterService>();
        services.AddSingleton<MappingService>();
        services.AddSingleton<ITransformPipeline, TransformPipeline>();
        services.AddSingleton<ITransformService<string>, TransformService>();
        services.AddSingleton<IJsonService, JsonService>();

        // Worker
        services.AddHostedService<TransformWorker>();
    });

await builder.Build().RunAsync();
