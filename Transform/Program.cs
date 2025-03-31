using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Transform.Controller;
using Transform.Kafka;
using Transform.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole(); //Console logging
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Kafka dependencies
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<IKafkaConsumer>(provider =>
            new KafkaConsumer("host.docker.internal:9092", "transform-group", "rawData"));

        // Transformation pipeline
        services.AddSingleton<MappingService>(); //Stateless mapper
        services.AddSingleton<ITransformPipeline, TransformPipeline>(); //Orchestrates mapping/filtering steps
        services.AddSingleton<ITransformService<string>, TransformService>(); //Coordinates entire process

        // Background service to start consuming
        services.AddHostedService<TransformController>();
    });

await builder.Build().RunAsync();
