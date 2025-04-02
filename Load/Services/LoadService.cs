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

        var writer = _writerResolver.Resolve(payload.LoadTargetConfig.TargetType, _services)
            ?? throw new InvalidOperationException($"No writer found for '{payload.LoadTargetConfig.TargetType}'");

        await writer.WriteAsync(payload.LoadTargetConfig.TargetInfo, payload.Data);

    }

}