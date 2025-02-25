namespace Speck.DurableMessaging.Inbox;

internal class Inbox(
    IInboxMessageRepository repository,
    InboxMessageFactory factory,
    InboxMessageTables tables,
    InboxSignalScope signals) : IInbox
{
    public async Task InsertAsync(object message)
    {
        var inboxMessage = factory.Create(message);

        var inboxTable = tables.GetInboxTable(message.GetType());
        
        await repository.InsertAsync(inboxMessage, inboxTable);
        
        signals.Signal(inboxTable);
    }
}