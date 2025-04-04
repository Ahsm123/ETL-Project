using ETL.Domain.Events;
using Load.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Load.Services;

public class LoadHandler : ILoadHandler
{
    private readonly ITargetWriterResolver _targetWriterResolver;
    private readonly IServiceProvider _serviceProvider;

    public LoadHandler(
        ITargetWriterResolver targetWriterResolver, 
        IServiceProvider serviceProvider)
    {
        _targetWriterResolver = targetWriterResolver;
        _serviceProvider = serviceProvider;
    }
    public async Task HandleAsync(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = JsonSerializer.Deserialize<TransformedEvent>(json, options)
                      ?? throw new InvalidOperationException("Invalid payload.");

        var targetInfo = payload.LoadTargetConfig.TargetInfo;

        var writer = _targetWriterResolver.Resolve(targetInfo.GetType(), _serviceProvider)
                     ?? throw new InvalidOperationException($"No writer found for type '{targetInfo.GetType()}'");

        await writer.WriteAsync(targetInfo, payload.Data);
    }
}
