namespace Speck.DurableMessaging.Mailbox;

internal interface IMailboxMessageRepository
{
    Task<MailboxMessage> GetMailboxMessageAsync(Guid mailboxMessageId, string mailboxMessageTable);
    
    Task<IReadOnlyCollection<MailboxMessage>> GetMailboxMessagesAsync(string mailboxMessageTable, int count);
    
    Task<IReadOnlyCollection<MailboxMessage>> GetMailboxMessagesAsync(IEnumerable<Guid> mailboxMessageIds, string mailboxMessageTable);

    Task InsertAsync(MailboxMessage mailboxMessage, string mailboxMessageTable);
    
    Task LockMailboxMessagesAsync(IReadOnlyCollection<MailboxMessage> messages, string mailboxMessageTable, DateTime lockedUntil);
    
    Task ProcessMailboxMessageAsync(MailboxMessage message, string mailboxMessageTable);
    
    Task ProcessMailboxMessagesAsync(IReadOnlyCollection<MailboxMessage> messages, string mailboxMessageTable);
}