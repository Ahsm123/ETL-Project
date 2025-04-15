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
    private readonly ILogger<EventDispatcher> _logger;
    private readonly Dictionary<string, string> _eventTopics;

    public EventDispatcher(
        IMessagePublisher publisher,
        IOptions<EventRoutingOptions> routingOptions,
        ILogger<EventDispatcher> logger)
    {
        _publisher = publisher;
        _logger = logger;
        _eventTopics = routingOptions.Value.EventTopics;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event)
    {
        var eventName = typeof(TEvent).Name;

        if (!_eventTopics.TryGetValue(eventName, out var topic))
        {
            _logger.LogWarning("No topic mapping found for event type {EventType}", eventName);
            return;
        }

        var payload = JsonSerializer.Serialize(@event);
        var key = Guid.NewGuid().ToString();

        await _publisher.PublishAsync(topic, key, payload);

        _logger.LogInformation("Dispatched {EventType} to topic {Topic} with key {Key}", eventName, topic, key);
    }
}
