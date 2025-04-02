using ETL.Domain.Model.DTOs;
using ETL.Domain.Model.TargetInfo;
using ETL.Domain.Model;
using Load.Writers;
using System.Text.Json;
using ETL.Domain.Utilities;

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
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var targetType = root.GetProperty("Load").GetProperty("TargetType").GetString()
                        ?? throw new InvalidOperationException("TargetType is missing.");

        var targetTypeResolved = TargetTypeMapper.GetTargetInfoType(targetType);

        var targetInfo = (TargetInfoBase)(JsonSerializer.Deserialize(
            root.GetProperty("Load").GetProperty("TargetInfo").GetRawText(),
            targetTypeResolved,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? throw new InvalidOperationException("Failed to deserialize TargetInfo."));

        var payload = new ProcessedPayload
        {
            PipelineId = root.GetProperty("PipelineId").GetString(),
            SourceType = root.GetProperty("SourceType").GetString(),
            Load = new LoadSettings
            {
                TargetType = targetType,
                TargetInfo = targetInfo
            },
            Data = JsonSerializer.Deserialize<Dictionary<string, object>>(
                root.GetProperty("Data").GetRawText()
            ) ?? new Dictionary<string, object>()
        };

        var writer = _writerResolver.Resolve(targetType, _services);
        if (writer == null)
            throw new InvalidOperationException($"No writer found for target type '{targetType}'.");

        await writer.WriteAsync(payload.Load.TargetInfo, payload.Data);
    }
}
