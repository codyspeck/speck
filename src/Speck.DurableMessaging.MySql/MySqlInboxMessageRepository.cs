using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging.MySql;

internal class MySqlInboxMessageRepository(MySqlConnection connection) : IInboxMessageRepository
{
    public async Task<InboxMessage> GetInboxMessageAsync(Guid inboxMessageId, string inboxMessageTable)
    {
        return await connection.QueryFirstAsync<InboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {inboxMessageTable}
            WHERE id = @inboxMessageId
            FOR UPDATE;
            """,
            new
            {
                inboxMessageId
            });
    }

    public async Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count)
    {
        var inboxMessages = await connection.QueryAsync<InboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {inboxMessageTable}
            WHERE processed_at IS NULL AND (locked_until IS NULL OR locked_until < NOW())
            LIMIT @count
            FOR UPDATE SKIP LOCKED;
            """,
            new { count });

        return inboxMessages.ToArray();
    }

    public async Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(
        IEnumerable<Guid> inboxMessageIds,
        string inboxMessageTable)
    {
        return (await connection.QueryAsync<InboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {inboxMessageTable}
            WHERE id IN @inboxMessageIds
            FOR UPDATE;
            """,
            new
            {
                inboxMessageIds
            }))
            .ToArray();
    }

    public async Task InsertAsync(InboxMessage inboxMessage, string inboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            INSERT INTO {inboxMessageTable} (id, type, content, message_key, created_at, locked_until)
            VALUES (@id, @type, @content, @message_key, @created_at, @locked_until);
            """,
            new
            {
                id = inboxMessage.Id,
                type = inboxMessage.Type,
                content = inboxMessage.Content,
                message_key = inboxMessage.MessageKey,
                created_at = inboxMessage.CreatedAt,
                locked_until = inboxMessage.LockedUntil
            });
    }

    public async Task LockInboxMessagesAsync(
        IReadOnlyCollection<InboxMessage> messages,
        string inboxMessageTable,
        DateTime lockedUntil)
    {
        if (messages.Count == 0)
            return;
        
        await connection.ExecuteAsync(
            $"""
            UPDATE {inboxMessageTable}
            SET locked_until = @lockedUntil
            WHERE id IN @ids; 
            """,
            new
            {
                ids = messages.Select(x => x.Id),
                lockedUntil
            });
    }

    public async Task ProcessInboxMessageAsync(InboxMessage message, string inboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {inboxMessageTable}
            SET processed_at = NOW()
            WHERE id = @inboxMessageId;
            """,
            new
            {
                inboxMessageId = message.Id,
            });
    }

    public async Task ProcessInboxMessagesAsync(IReadOnlyCollection<InboxMessage> messages, string inboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {inboxMessageTable}
            SET processed_at = NOW()
            WHERE id IN @inboxMessageIds;
            """,
            new
            {
                inboxMessageIds = messages.Select(x => x.Id)
            });
    }
}