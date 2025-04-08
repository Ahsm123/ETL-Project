using Load.Interfaces;
using Load.Messaging;
using Load.Services;
using Load.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    // Dynamisk registrering af alle ITargetWriter-implementeringer med constructor-injektion
    var writerTypes = typeof(ITargetWriter).Assembly
        .GetTypes()
        .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) &&
                    t is { IsClass: true, IsAbstract: false });

    foreach (var type in writerTypes)
    {
        services.AddSingleton(typeof(ITargetWriter), serviceProvider =>
            ActivatorUtilities.CreateInstance(serviceProvider, type));
    }

    // Registrér resten af services
    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();
    services.AddSingleton<IMessageListener, KafkaMessageListener>();
    services.AddSingleton<ILoadHandler, LoadHandler>();
    services.AddHostedService<LoadWorker>();
});

await builder.Build().RunAsync();
