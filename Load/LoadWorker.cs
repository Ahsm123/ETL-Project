using Load.Kafka.Interfaces;
using Load.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Load;

public class LoadWorker : BackgroundService
{
    private readonly ILogger<LoadWorker> _logger;
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly ILoadHandler _loadHandler;

    public LoadWorker(
        ILogger<LoadWorker> logger,
        IKafkaConsumer kafkaConsumer,
        ILoadHandler loadHandler)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _loadHandler = loadHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LoadWorker is starting...");

        await _kafkaConsumer.StartAsync(async (message) =>
        {
            try
            {
                await _loadHandler.HandleAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle message.");
            }
        }, stoppingToken);

        _logger.LogInformation("LoadWorker is stopping...");
    }
}

