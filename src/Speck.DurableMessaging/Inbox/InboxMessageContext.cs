namespace Speck.DurableMessaging.Inbox;

public class InboxMessageContext(Guid inboxMessageId, string inboxMessageTable, object message)
{
    public Guid InboxMessageId { get; } = inboxMessageId;

    public string InboxMessageTable { get; } = inboxMessageTable;

    public object Message { get; } = message;
}