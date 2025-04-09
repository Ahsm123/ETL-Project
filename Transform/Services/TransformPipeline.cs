using ETL.Domain.Events;
using ETL.Domain.Rules;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Transform.Interfaces;

namespace Transform.Services;

public class TransformPipeline : ITransformPipeline
{
    private readonly FilterService _filterService;
    private readonly MappingService _mappingService;
    private readonly ILogger<TransformPipeline> _logger;

    public TransformPipeline(
        MappingService mappingService,
        FilterService filterService,
        ILogger<TransformPipeline> logger) // Inject logger
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
            if (!_filterService.ShouldInclude(input.Data, filters))
            {
                _logger.LogInformation("Event {Id} was excluded by filters", input.Id);
                return null;
            }

            var mappings = input.TransformConfig?.Mappings ?? new();
            var mapped = _mappingService.Apply(input.Data, mappings);

            var json = JsonSerializer.Serialize(mapped);
            _logger.LogDebug("Mapped data for event {Id}: {Json}", input.Id, json);

            var jsonElement = JsonDocument.Parse(json).RootElement.Clone();

            return new TransformedEvent
            {
                PipelineId = input.Id,
                LoadTargetConfig = input.LoadTargetConfig,
                Data = mapped
            };
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON error while processing event {Id}", input.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing event {Id}", input.Id);
        }

        return null;
    }
}
