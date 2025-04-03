using Load.Kafka;
using Load.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Load;

public class LoadWorker : BackgroundService
{
    private readonly ILogger<LoadWorker> _logger;
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly LoadService _loadService;

    public LoadWorker(
        ILogger<LoadWorker> logger,
        IKafkaConsumer kafkaConsumer,
        LoadService loadService)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _loadService = loadService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LoadWorker is starting...");

        await _kafkaConsumer.StartAsync(async (message) =>
        {
            try
            {
                await _loadService.HandleEventAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle message.");
            }
        }, stoppingToken);

        _logger.LogInformation("LoadWorker is stopping...");
    }
}
