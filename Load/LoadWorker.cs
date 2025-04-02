using Load.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load;

public class LoadWorker : BackgroundService
{
    private readonly ILogger<LoadWorker> _logger;
    private readonly LoadService _loadService;

    public LoadWorker(ILogger<LoadWorker> logger, LoadService loadService)
    {
        _logger = logger;
        _loadService = loadService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LoadWorker is running...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Simulate Kafka message consumption
                var json = await SimulateKafkaReceive();

                await _loadService.HandleMessageAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message in LoadWorker");
            }

            await Task.Delay(1000, stoppingToken); // 
        }

        _logger.LogInformation("LoadWorker is stopping...");
    }

    private Task<string> SimulateKafkaReceive()
    {
        // Simulate a JSON string from Kafka (replace with actual Kafka consumer)
        var dummyJson = """
        {
            "PipelineId": "pipeline_001",
            "SourceType": "api",
            "Load": {
                "TargetType": "mssql",
                "TargetInfo": {
                    "ConnectionString": "Server=db;User Id=admin;Password=secret;",
                    "TableName": "approved_payments",
                    "UseBulkInsert": true
                }
            },
            "Data": {
                "id": "123",
                "total": 5500
            }
        }
        """;

        return Task.FromResult(dummyJson);
    }
}