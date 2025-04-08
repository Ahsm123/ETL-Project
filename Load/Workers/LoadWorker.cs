using Load.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Load.Workers;

public class LoadWorker : BackgroundService
{
    private readonly ILogger<LoadWorker> _logger;
    private readonly IMessageListener _messageListener;
    private readonly ILoadHandler _handler;

    public LoadWorker(
        ILogger<LoadWorker> logger,
        IMessageListener messageListener,
        ILoadHandler handler)
    {
        _logger = logger;
        _messageListener = messageListener;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LoadWorker starting...");

        await _messageListener.ListenAsync(async (msg) =>
        {
            try
            {
                await _handler.HandleAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle message.");
            }
        }, stoppingToken);

        _logger.LogInformation("LoadWorker stopping...");
    }
}

