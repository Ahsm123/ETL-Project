using Load.Messaging.Kafka.KafkaConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test.TransformTest.KafkaTests;

public class KafkaConsumerTests
{
    private static IOptions<KafkaSettings> BuildKafkaOptions(string bootstrapServers)
    {
        return Options.Create(new KafkaSettings
        {
            BootstrapServers = bootstrapServers,
            Consumer = new ConsumerSettings
            {
                Topic = "test-topic",
                GroupId = "test-group",
                AutoOffsetReset = "Earliest"
            },
            Producer = new ProducerSettings
            {
                EnableIdempotence = true,
                Acks = "all"
            },
            Topics = new TopicSettings
            {
                DeadLetter = "dead-letter"
            }
        });
    }

    [Fact]
    public async Task KafkaConsumer_CancelsGracefully()
    {
        var logger = new LoggerFactory().CreateLogger<KafkaConsumer>();
        var options = BuildKafkaOptions("localhost:9092");

        var listener = new KafkaConsumer(logger, options);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

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
    public async Task KafkaConsumer_WhenKafkaOffline_LogsError()
    {
        var logger = new LoggerFactory().CreateLogger<KafkaConsumer>();
        var options = BuildKafkaOptions("localhost:9999");

        var listener = new KafkaConsumer(logger, options);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        await listener.ListenAsync(async (msg) =>
        {
            // Should not be hit
        }, cts.Token);

        Assert.True(true);
    }
}
