namespace EventHandlerService;

public interface IEventProcessor
{
    Task ProcessEventsAsync(CancellationToken cancellationToken);
}