using ExtractAPI.Interfaces;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using ExtractAPI.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExtractAPI.Messaging;

public class EventDispatcher : IEventDispatcher
{
    private readonly IMessagePublisher _publisher;
    private readonly IConfiguration _configuration;

    public EventDispatcher(IMessagePublisher publisher, IConfiguration configuration)
    {
        _publisher = publisher;
        _configuration = configuration;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event)
    {
        if (@event is DataExtractedEvent dataEvent)
        {
            var json = JsonSerializer.Serialize(dataEvent.ExtractedEvent);
            var topic = _configuration["Kafka:RawDataTopic"] ?? "rawData";

            await _publisher.PublishAsync(topic, Guid.NewGuid().ToString(), json);
        }
    }
}
