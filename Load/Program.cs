using Load;
using Load.Services;
using Load.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    // Register writers
    services.AddSingleton<ITargetWriter, MsSqlTargetWriter>(); 

    // Register services
    services.AddSingleton<ITargetWriterResolver, TargetWriterResolver>();
    services.AddSingleton<LoadService>();

    // Register background worker
    services.AddHostedService<LoadWorker>();
});

await builder.Build().RunAsync();
