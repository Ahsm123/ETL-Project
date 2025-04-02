using ETL.Domain.Config;
using ETL.Domain.Model.DTOs;
using ETL.Domain.Model.TargetInfo;
using ETL.Domain.Utilities;
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

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var pipelineId = root.GetProperty("pipelineId").GetString()!;
        var sourceType = root.GetProperty("sourceType").GetString()!;
        var load = root.GetProperty("load");
        var targetType = load.GetProperty("targetType").GetString()!;
        var targetInfoRaw = load.GetProperty("targetInfo").GetRawText();

        var targetTypeResolved = TargetTypeMapper.GetTargetInfoType(targetType);
        if (targetTypeResolved == null)
            throw new InvalidOperationException($"Unrecognized TargetType: {targetType}");

        var targetInfo = (TargetInfoBase)JsonSerializer.Deserialize(
            targetInfoRaw,
            targetTypeResolved,
            options
        )!;

        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(
            root.GetProperty("data").GetRawText(), options
        )!;

        var payload = new ProcessedPayload
        {
            PipelineId = pipelineId,
            SourceType = sourceType,
            Load = new LoadTargetConfig
            {
                TargetType = targetType,
                TargetInfo = targetInfo
            },
            Data = data
        };

        var writer = _writerResolver.Resolve(targetType, _services);
        if (writer == null)
            throw new InvalidOperationException($"No writer found for target type '{targetType}'.");

        await writer.WriteAsync(payload.Load.TargetInfo, payload.Data);
    }


}
