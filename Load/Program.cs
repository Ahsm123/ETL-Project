using Load;
using Load.Kafka;
using Load.Kafka.Interfaces;
using Load.Services;
using Load.Services.Interfaces;
using Load.Writers;
using Load.Writers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    var writerTypes = typeof(ITargetWriter).Assembly
        .GetTypes()
        .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) &&
                    t is { IsClass: true, IsAbstract: false });

    foreach (var type in writerTypes)
    {
        services.AddSingleton(typeof(ITargetWriter), type);
    }

    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();

    services.AddSingleton<IKafkaConsumer, KafkaProcessedPayloadConsumer>();
    services.AddSingleton<ILoadHandler, LoadHandler>();

    services.AddHostedService<LoadWorker>();
});

await builder.Build().RunAsync();
