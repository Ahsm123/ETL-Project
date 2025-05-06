namespace ExtractAPI.Messaging.Interfaces;

public interface IEventRouter
{
    Task DispatchAsync<TEvent>(TEvent @event);
}
