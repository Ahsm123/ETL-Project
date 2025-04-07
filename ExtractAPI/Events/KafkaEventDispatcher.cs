
using ExtractAPI.Events.Interfaces;
using ExtractAPI.ExtractedEvents;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExtractAPI.Events;

public class KafkaEventDispatcher : IEventDispatcher
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly KafkaSettings _settings;

    public KafkaEventDispatcher(IKafkaProducer kafkaProducer, IOptions<KafkaSettings> options)
    {
        _kafkaProducer = kafkaProducer;
        _settings = options.Value;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event)
    {
        if (@event is DataExtractedEvent dataEvent)
        {
            var json = JsonSerializer.Serialize(dataEvent.ExtractedEvent);
            await _kafkaProducer.PublishAsync(_settings.RawDataTopic, Guid.NewGuid().ToString(), json);
        }
    }
}
