using ETL.Domain.Model.DTOs;
using System.Text.Json;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly FilterService _filterService;
    private readonly MappingService _mappingService;

    public TransformPipeline(
        MappingService mappingService,
        FilterService filterService)
    {
        _mappingService = mappingService;
        _filterService = filterService;
    }

    public TransformPayload Execute(ExtractedPayload input)
    {
        if (input.Transform?.Filters != null && !_filterService.ShouldInclude(input.Data, input.Transform.Filters))
        {
            throw new InvalidOperationException($"Payload {input.Id} did not pass filters");
        }

        var mapped = _mappingService.Apply(input.Data, input.Transform?.Mappings ?? new());
        var json = JsonSerializer.Serialize(mapped);
        var jsonElement = JsonDocument.Parse(json).RootElement.Clone();

        return new TransformPayload
        {
            Id = input.Id,
            SourceType = input.SourceType,
            Load = input.Load,
            Data = jsonElement
        };
    }

}
