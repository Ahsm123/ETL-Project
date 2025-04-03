using Load;
using Load.Kafka;
using Load.Services;
using Load.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
_ = typeof(MsSqlTargetWriter).Assembly; // force-load writer assembly
builder.ConfigureServices(services =>
{
    // Register writers
    services.AddSingleton<MsSqlTargetWriter>();
    services.AddSingleton<ITargetWriter, MsSqlTargetWriter>();
    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();

    // Register services
    services.AddSingleton<IKafkaConsumer, KafkaProcessedPayloadConsumer>();
    services.AddSingleton<LoadService>();

    // Register background worker
    services.AddHostedService<LoadWorker>();
});

await builder.Build().RunAsync();
