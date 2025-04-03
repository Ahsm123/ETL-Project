using ETL.Domain.Events;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly MappingService _mappingService;

    public TransformPipeline(MappingService mappingService)
    {
        _mappingService = mappingService;
    }

    public TransformedEvent Execute(ExtractedEvent input)
    {
        var mapped = _mappingService.Apply(input.Data, input.TransformConfig?.Mappings ?? new());

        return new TransformedEvent
        {
            PipelineId = input.Id,
            LoadTargetConfig = input.LoadTargetConfig,
            Data = mapped
        };
    }
}
