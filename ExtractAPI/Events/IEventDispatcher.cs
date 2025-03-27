namespace ExtractAPI.Events;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event);
}
