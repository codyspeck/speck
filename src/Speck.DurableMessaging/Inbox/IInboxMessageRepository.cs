namespace Speck.DurableMessaging.Inbox;

internal interface IInboxMessageRepository
{
    Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count);
}