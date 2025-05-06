using ETL.Domain.Events;
using ETL.Domain.Rules;
using Microsoft.Extensions.Logging;
using Transform.Services.Interfaces;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly FilterService _filterService;
    private readonly MappingService _mappingService;
    private readonly ILogger<TransformPipeline> _logger;

    public TransformPipeline(
        MappingService mappingService,
        FilterService filterService,
        ILogger<TransformPipeline> logger)
    {
        _mappingService = mappingService;
        _filterService = filterService;
        _logger = logger;
    }

    public TransformedEvent? Run(ExtractedEvent input)
    {
        try
        {
            var filters = input.TransformConfig?.Filters ?? Enumerable.Empty<FilterRule>();
            if (!_filterService.ShouldInclude(input.Record, filters))
            {
                _logger.LogInformation("Event {Id} was excluded by filters", input.PipelineId);
                return null;
            }

            var mappings = input.TransformConfig?.Mappings ?? new();
            var mapped = _mappingService.Apply(input.Record, mappings);

            return new TransformedEvent
            {
                PipelineId = input.PipelineId,
                LoadTargetConfig = input.LoadTargetConfig,
                Record = mapped
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing event {Id}", input.PipelineId);
            return null;
        }
    }
}
