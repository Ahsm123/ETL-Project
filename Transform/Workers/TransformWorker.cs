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
        _logger.LogInformation("EnableDeadLetter config: {EnableDeadLetter}, Topic: {DeadLetterTopic}", _enableDeadLetter, _deadLetterTopic);


        await _listener.ListenAsync(async (message) =>
        {
            try
            {
                _logger.LogDebug("Received message: {Message}", message);

                ExtractedEvent? payload;
                try
                {
                    payload = _jsonService.Deserialize<ExtractedEvent>(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize ExtractedEvent: {Message}", message);
                    await SendToDeadLetter(message);
                    return;
                }

                if (payload == null)
                {
                    _logger.LogWarning("Deserialized payload is null");
                    await SendToDeadLetter(message);
                    return;
                }

                var transformed = await _transformService.TransformDataAsync(payload);

                if (string.IsNullOrWhiteSpace(transformed) || transformed == "{}")
                {
                    _logger.LogInformation("Skipping filtered or empty payload with ID {Id}", payload.PipelineId);
                    return;
                }

                await _publisher.PublishAsync("processedData", Guid.NewGuid().ToString(), transformed);
                _logger.LogInformation("Published transformed payload with ID {Id}", payload.PipelineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing message");
                await SendToDeadLetter(message);
            }
        }, stoppingToken);

    }

    private async Task SendToDeadLetter(string originalMessage, string error = "UnknownError")
    {
        if (!_enableDeadLetter)
            return;

        var envelope = new DeadLetterEnvelope
        {
            OriginalPayload = originalMessage,
            Error = error,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var wrapped = _jsonService.Serialize(envelope);
            await _publisher.PublishAsync(_deadLetterTopic, Guid.NewGuid().ToString(), wrapped);
            _logger.LogWarning("Message sent to dead-letter topic: {Topic}", _deadLetterTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to dead-letter topic");
        }
    }

    public class DeadLetterEnvelope
    {
        public string OriginalPayload { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }



}
