using ETL.Domain.Model.DTOs;
using System.Text.Json;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly MappingService _mappingService;

    public TransformPipeline(MappingService mappingService)
    {
        _mappingService = mappingService;
    }

    public TransformPayload Execute(ExtractedPayload input)
    {
        var mappedData = _mappingService.Apply(input.Data, input.Transform?.Mappings ?? new());

        var json = JsonSerializer.Serialize(mappedData);
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
