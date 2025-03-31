using ETL.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Transform.Controller;
using Transform.Kafka;
using Transform.Services;
using Transform.Strategy;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole(); // ✅ enables proper logging to Output & Terminal
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Configuration example (optional)
        var config = hostContext.Configuration;

        // Register dependencies
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<IKafkaConsumer>(provider =>
        {
            // TODO: Inject your bootstrapServers, groupId and topic properly
            return new KafkaConsumer("host.docker.internal:9092", "transform-group", "rawData");
        });

        services.AddSingleton<MappingStrategy>(provider => new MappingStrategy(new List<FieldMapping>()));

        services.AddSingleton<ITransformService<string>, TransformService>();

        // Register Background Worker
        services.AddHostedService<TransformController>();
    });

await builder.Build().RunAsync();