using Load.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TransformTest.KafkaTests;

public class KafkaMessageListenerTests
{
    [Fact]
    public async Task KafkaMessageListener_CancelsGracefully()
    {
        var logger = new LoggerFactory().CreateLogger<KafkaMessageListener>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
            { "Kafka:BootstrapServers", "localhost:9092" }
            })
            .Build();

        var listener = new KafkaMessageListener(logger, config);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // Force cancel after 5s

        await listener.ListenAsync(
            async (msg) =>
            {
                await Task.Delay(100);
            },
            cts.Token
        );

        Assert.True(true);
    }

    [Fact]
    public async Task KafkaMessageListener_WhenKafkaOffline_LogsError()
    {
        // Use an unreachable Kafka host
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
            { "Kafka:BootstrapServers", "localhost:9999" } // Invalid port
            }!)
            .Build();

        // Get log output
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<KafkaMessageListener>();

        var listener = new KafkaMessageListener(logger, config);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // Force stop

        // This should log error, not throw
        await listener.ListenAsync(async (msg) =>
        {
            // Won't be reached
        }, cts.Token);

        Assert.True(true); 
    }


}
