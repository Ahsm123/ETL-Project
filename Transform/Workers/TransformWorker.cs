
using ETL.Domain.Events;
using ETL.Domain.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Transform.Interfaces;

namespace Transform.Workers;

public class TransformWorker : BackgroundService
{
    private readonly IMessageListener _listener;
    private readonly IMessagePublisher _publisher;
    private readonly ITransformService<string> _transformService;
    private readonly ILogger<TransformWorker> _logger;

    public TransformWorker(
        IMessageListener listener,
        IMessagePublisher publisher,
        ITransformService<string> transformService,
        ILogger<TransformWorker> logger)
    {
        _listener = listener;
        _publisher = publisher;
        _transformService = transformService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TransformWorker started");

        await _listener.ListenAsync(async (message) =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<ExtractedEvent>(message, JsonOptionsFactory.Default);
                if (payload == null)
                {
                    _logger.LogWarning("Received null or invalid payload");
                    return;
                }

                var transformed = await _transformService.TransformDataAsync(payload);
                await _publisher.PublishAsync("processedData", Guid.NewGuid().ToString(), transformed);

                _logger.LogInformation("Processed payload with ID {Id}", payload.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }, stoppingToken);
    }
}





