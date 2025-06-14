﻿using ExtractAPI.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExtractAPI.Messaging;

public class EventRouter : IEventRouter
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<EventRouter> _logger;
    private readonly Dictionary<string, string> _eventTopics;

    public EventRouter(
        IMessagePublisher publisher,
        IOptions<EventRoutingOptions> routingOptions,
        ILogger<EventRouter> logger)
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
