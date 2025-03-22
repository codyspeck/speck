using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Mailbox;

namespace Speck.DurableMessaging.MySql;

internal class MySqlMailboxMessageRepository(MySqlConnection connection) : IMailboxMessageRepository
{
    public async Task<MailboxMessage> GetMailboxMessageAsync(Guid mailboxMessageId, string mailboxMessageTable)
    {
        return await connection.QueryFirstAsync<MailboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {mailboxMessageTable}
            WHERE id = @mailboxMessageId
            FOR UPDATE;
            """,
            new
            {
                mailboxMessageId
            });
    }

    public async Task<IReadOnlyCollection<MailboxMessage>> GetMailboxMessagesAsync(string mailboxMessageTable, int count)
    {
        var mailboxMessages = await connection.QueryAsync<MailboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {mailboxMessageTable}
            WHERE processed_at IS NULL AND (locked_until IS NULL OR locked_until < NOW())
            LIMIT @count
            FOR UPDATE SKIP LOCKED;
            """,
            new { count });

        return mailboxMessages.ToArray();
    }

    public async Task<IReadOnlyCollection<MailboxMessage>> GetMailboxMessagesAsync(
        IEnumerable<Guid> mailboxMessageIds,
        string mailboxMessageTable)
    {
        return (await connection.QueryAsync<MailboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {mailboxMessageTable}
            WHERE id IN @mailboxMessageIds
            FOR UPDATE;
            """,
            new
            {
                mailboxMessageIds
            }))
            .ToArray();
    }

    public async Task InsertAsync(MailboxMessage mailboxMessage, string mailboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            INSERT INTO {mailboxMessageTable} (id, type, content, message_key, created_at, locked_until)
            VALUES (@id, @type, @content, @message_key, @created_at, @locked_until);
            """,
            new
            {
                id = mailboxMessage.Id,
                type = mailboxMessage.Type,
                content = mailboxMessage.Content,
                message_key = mailboxMessage.MessageKey,
                created_at = mailboxMessage.CreatedAt,
                locked_until = mailboxMessage.LockedUntil
            });
    }

    public async Task LockMailboxMessagesAsync(
        IReadOnlyCollection<MailboxMessage> messages,
        string mailboxMessageTable,
        DateTime lockedUntil)
    {
        if (messages.Count == 0)
            return;
        
        await connection.ExecuteAsync(
            $"""
            UPDATE {mailboxMessageTable}
            SET locked_until = @lockedUntil
            WHERE id IN @ids; 
            """,
            new
            {
                ids = messages.Select(x => x.Id),
                lockedUntil
            });
    }

    public async Task ProcessMailboxMessageAsync(MailboxMessage message, string mailboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {mailboxMessageTable}
            SET processed_at = NOW()
            WHERE id = @mailboxMessageId;
            """,
            new
            {
                mailboxMessageId = message.Id,
            });
    }

    public async Task ProcessMailboxMessagesAsync(IReadOnlyCollection<MailboxMessage> messages, string mailboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {mailboxMessageTable}
            SET processed_at = NOW()
            WHERE id IN @mailboxMessageIds;
            """,
            new
            {
                mailboxMessageIds = messages.Select(x => x.Id)
            });
    }
}