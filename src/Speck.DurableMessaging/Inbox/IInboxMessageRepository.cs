namespace Speck.DurableMessaging.Inbox;

public interface IInboxMessageRepository
{
    Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count);
}