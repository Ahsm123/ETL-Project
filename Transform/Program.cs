using ETL.Domain.JsonHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Transform.Interfaces;
using Transform.Messaging.Kafka.KafkaConfig;
using Transform.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<KafkaSettings>(context.Configuration.GetSection("Kafka"));

        // Register Kafka
        services.AddSingleton<IMessageListener, KafkaConsumer>();
        services.AddSingleton<IMessagePublisher, KafkaProducer>();

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