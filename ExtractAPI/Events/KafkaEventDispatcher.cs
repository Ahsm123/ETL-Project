
using ExtractAPI.Events.Interfaces;
using ExtractAPI.ExtractedEvents;
using ExtractAPI.Kafka.Interfaces;
using System.Text.Json;

namespace ExtractAPI.Events;

public class KafkaEventDispatcher : IEventDispatcher
{
    private readonly IKafkaProducer _kafkaProducer;

    public KafkaEventDispatcher(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }
    public async Task DispatchAsync<TEvent>(TEvent @event)
    {
        if (@event is DataExtractedEvent dataEvent)
        {
            var json = JsonSerializer.Serialize(dataEvent.ExtractedEvent);
            await _kafkaProducer.PublishAsync("rawData", Guid.NewGuid().ToString(), json);
        }
    }
}
