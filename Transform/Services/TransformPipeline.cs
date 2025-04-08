using ETL.Domain.Events;
using ETL.Domain.Rules;
using System.Text.Json;
using Transform.Interfaces;

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

    public TransformedEvent? Run(ExtractedEvent input)
    {
        if (!_filterService.ShouldInclude(
            input.Data,
            input.TransformConfig?.Filters ?? Enumerable.Empty<FilterRule>()))
        {
            return null;
        }

        var mapped = _mappingService.Apply(input.Data, input.TransformConfig?.Mappings ?? new());

        var json = JsonSerializer.Serialize(mapped);
        Console.WriteLine(json);
        var jsonElement = JsonDocument.Parse(json).RootElement.Clone();

        return new TransformedEvent
        {
            PipelineId = input.Id,
            LoadTargetConfig = input.LoadTargetConfig,
            Data = mapped
        };
    }
}
