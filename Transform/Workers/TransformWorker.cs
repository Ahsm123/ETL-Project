using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Transform.Interfaces;

namespace Transform.Workers;
public class TransformWorker : BackgroundService
{
    private readonly IMessageListener _listener;
    private readonly IMessagePublisher _publisher;
    private readonly ITransformService<string> _transformService;
    private readonly IJsonService _jsonService;
    private readonly ILogger<TransformWorker> _logger;
    private readonly bool _enableDeadLetter;
    private readonly string _deadLetterTopic;

    public TransformWorker(
        IMessageListener listener,
        IMessagePublisher publisher,
        ITransformService<string> transformService,
        IJsonService jsonService,
        ILogger<TransformWorker> logger,
        IConfiguration config)
    {
        _listener = listener;
        _publisher = publisher;
        _transformService = transformService;
        _jsonService = jsonService;
        _logger = logger;
        _enableDeadLetter = config.GetValue<bool>("Transform:EnableDeadLetter");
        _deadLetterTopic = config.GetValue<string>("Transform:DeadLetterTopic") ?? "dead-letter";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TransformWorker started");
        _logger.LogInformation("EnableDeadLetter: {EnableDeadLetter}, Topic: {Topic}", _enableDeadLetter, _deadLetterTopic);

        await _listener.ListenAsync(HandleMessageAsync, stoppingToken);
    }

    private async Task HandleMessageAsync(string message)
    {
        _logger.LogDebug("Received message: {Message}", message);

        var payload = TryDeserialize(message);
        if (payload == null)
        {
            await SendToDeadLetter(message, ErrorType.DeserializationError);
            return;
        }

        var transformed = await TryTransform(payload, message);
        if (string.IsNullOrWhiteSpace(transformed) || transformed == "{}")
        {
            _logger.LogInformation("Filtered/empty payload. Skipping. PipelineId: {PipelineId}", payload.PipelineId);
            return;
        }

        await TryPublish(transformed, payload.PipelineId, message);

    }

    private ExtractedEvent? TryDeserialize(string message)
    {
        try
        {
            return _jsonService.Deserialize<ExtractedEvent>(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Deserialization failed");
            return null;
        }
    }

    private async Task<string> TryTransform(ExtractedEvent payload, string originalMessage)
    {
        try
        {
            return await _transformService.TransformDataAsync(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transformation failed. PipelineId: {PipelineId}", payload.PipelineId);
            await SendToDeadLetter(originalMessage, ErrorType.TransformationError);
            return string.Empty;
        }
    }

    private async Task TryPublish(string transformed, string pipelineId, string originalMessage)
    {
        try
        {
            await _publisher.PublishAsync("processedData", Guid.NewGuid().ToString(), transformed);
            _logger.LogInformation("Published transformed message. PipelineId: {PipelineId}", pipelineId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Publishing failed. PipelineId: {PipelineId}", pipelineId);
            await SendToDeadLetter(originalMessage, ErrorType.PublishError);
        }
    }


    private async Task SendToDeadLetter(string originalMessage, ErrorType errorType)
    {
        if (!_enableDeadLetter)
            return;

        var envelope = new DeadLetterEnvelope
        {
            OriginalPayload = originalMessage,
            Error = errorType.ToString(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var wrapped = _jsonService.Serialize(envelope);
            await _publisher.PublishAsync(_deadLetterTopic, Guid.NewGuid().ToString(), wrapped);
            _logger.LogWarning("Dead-lettered message. Error: {Error}", errorType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send to dead-letter topic");
        }
    }

    private enum ErrorType
    {
        DeserializationError,
        NullPayload,
        TransformationError,
        PublishError,
        UnknownError
    }
}
