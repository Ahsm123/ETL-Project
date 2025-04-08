namespace ExtractAPI.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event);
}
