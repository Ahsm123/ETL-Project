using ETL.Domain.Model.DTOs;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly MappingService _mappingService;

    public TransformPipeline(MappingService mappingService)
    {
        _mappingService = mappingService;
    }

    public ProcessedPayload Execute(ExtractedPayload input)
    {
        var mapped = _mappingService.Apply(input.Data, input.Transform?.Mappings ?? new());

        return new ProcessedPayload
        {
            PipelineId = input.Id,
            SourceType = input.SourceType,
            Load = input.Load,
            Data = mapped
        };
    }
}
