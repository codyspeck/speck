namespace Speck.DurableMessaging.Inbox;

public interface IInboxMessageBatchHandler<in TMessage>
{
    Task HandleAsync(IReadOnlyCollection<TMessage> messages);
}