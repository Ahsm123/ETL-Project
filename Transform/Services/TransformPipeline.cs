using ETL.Domain.Events;

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

    public TransformedEvent Execute(ExtractedEvent input)
    {
        if (!_filterService.ShouldInclude(input.Data, input.Transform?.Filters ?? new()))
        {
            return null; // skip payload
        }

        var mapped = _mappingService.Apply(input.Data, input.Transform?.Mappings ?? new());
        var json = JsonSerializer.Serialize(mapped);
        Console.WriteLine(json);
        var jsonElement = JsonDocument.Parse(json).RootElement.Clone();

        return new TransformedEvent
        {
            PipelineId = input.Id,
            SourceType = input.SourceType,
            LoadTargetConfig = input.LoadTargetConfig,
            Data = mapped
        };
    }
}
