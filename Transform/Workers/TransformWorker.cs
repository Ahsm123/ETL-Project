using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transform.Messaging.Interfaces;
using Transform.Messaging.Kafka.KafkaConfig;
using Transform.Services.Interfaces;

public class TransformWorker : BackgroundService
{
    private readonly IMessageListener _listener;
    private readonly IMessagePublisher _publisher;
    private readonly ITransformService<string> _transformService;
    private readonly IJsonService _jsonService;
    private readonly ILogger<TransformWorker> _logger;
    private readonly KafkaSettings _settings;

    public TransformWorker(
        IMessageListener listener,
        IMessagePublisher publisher,
        ITransformService<string> transformService,
        IJsonService jsonService,
        ILogger<TransformWorker> logger,
        IOptions<KafkaSettings> options)
    {
        _listener = listener;
        _publisher = publisher;
        _transformService = transformService;
        _jsonService = jsonService;
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TransformWorker started. Dead-letter enabled: {Enabled}", _settings.Topics.EnableDeadLetter);
        await _listener.ListenAsync(HandleMessageAsync, stoppingToken);
    }

    private async Task HandleMessageAsync(string message)
    {
        var payload = TryDeserialize(message);
        if (payload == null)
        {
            await SendToDeadLetter(message, "DeserializationError");
            return;
        }

        try
        {
            var transformed = await _transformService.TransformDataAsync(payload);
            if (!string.IsNullOrWhiteSpace(transformed))
            {
                await _publisher.PublishAsync("processedData", Guid.NewGuid().ToString(), transformed);
                _logger.LogInformation("Transformed message published.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transformation failed");
            await SendToDeadLetter(message, "TransformationError");
        }
    }

    private ExtractedEvent? TryDeserialize(string json)
    {
        try
        {
            return _jsonService.Deserialize<ExtractedEvent>(json);
        }
        catch
        {
            return null;
        }
    }

    private async Task SendToDeadLetter(string original, string reason)
    {
        if (!_settings.Topics.EnableDeadLetter) return;

        var envelope = new DeadLetterEnvelope
        {
            Timestamp = DateTime.UtcNow,
            Error = reason,
            OriginalPayload = original
        };

        var json = _jsonService.Serialize(envelope);
        await _publisher.PublishAsync(_settings.Topics.DeadLetterTopic, Guid.NewGuid().ToString(), json);
        _logger.LogWarning("Message sent to dead-letter topic.");
    }
}
