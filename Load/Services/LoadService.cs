using ETL.Domain.Events;
using Load.Writers;
using System.Text.Json;

namespace Load.Services;

public class LoadService
{
    private readonly ITargetWriterResolver _writerResolver;
    private readonly IServiceProvider _services;

    public LoadService(ITargetWriterResolver writerResolver, IServiceProvider services)
    {
        _writerResolver = writerResolver;
        _services = services;
    }

    public async Task HandleMessageAsync(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = JsonSerializer.Deserialize<TransformedEvent>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize payload.");

        // Use type directly from $type discriminator via System.Text.Json
        var targetInfo = payload.LoadTargetConfig.TargetInfo;

        var writer = _writerResolver.Resolve(targetInfo.GetType(), _services)
            ?? throw new InvalidOperationException($"No writer found for type '{targetInfo.GetType().Name}'");

        await writer.WriteAsync(targetInfo, payload.Data);
    }
}