using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Outbox;

namespace Speck.DurableMessaging.MySql;

internal class MySqlOutboxMessageRepository(MySqlConnection connection) : IOutboxMessageRepository
{
    public async Task<OutboxMessage> GetOutboxMessageAsync(Guid outboxMessageId, string outboxMessageTable)
    {
        return await connection.QueryFirstAsync<OutboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {outboxMessageTable}
            WHERE id = @outboxMessageId
            FOR UPDATE;
            """,
            new
            {
                outboxMessageId
            });
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> GetOutboxMessagesAsync(string outboxMessageTable, int count)
    {
        var outboxMessages = await connection.QueryAsync<OutboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {outboxMessageTable}
            WHERE processed_at IS NULL AND (locked_until IS NULL OR locked_until < NOW())
            LIMIT @count
            FOR UPDATE SKIP LOCKED;
            """,
            new { count });

        return outboxMessages.ToArray();
    }

    public async Task<IReadOnlyCollection<OutboxMessage>> GetOutboxMessagesAsync(
        IEnumerable<Guid> outboxMessageIds,
        string outboxMessageTable)
    {
        return (await connection.QueryAsync<OutboxMessage>(
            $"""
            SELECT id, type, content, created_at, locked_until, processed_at
            FROM {outboxMessageTable}
            WHERE id IN @outboxMessageIds
            FOR UPDATE;
            """,
            new
            {
                outboxMessageIds
            }))
            .ToArray();
    }

    public async Task InsertAsync(OutboxMessage outboxMessage, string outboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            INSERT INTO {outboxMessageTable} (id, type, content, message_key, created_at, locked_until)
            VALUES (@id, @type, @content, @message_key, @created_at, @locked_until);
            """,
            new
            {
                id = outboxMessage.Id,
                type = outboxMessage.Type,
                content = outboxMessage.Content,
                message_key = outboxMessage.MessageKey,
                created_at = outboxMessage.CreatedAt,
                locked_until = outboxMessage.LockedUntil
            });
    }

    public async Task LockOutboxMessagesAsync(
        IReadOnlyCollection<OutboxMessage> messages,
        string outboxMessageTable,
        DateTime lockedUntil)
    {
        if (messages.Count == 0)
            return;
        
        await connection.ExecuteAsync(
            $"""
            UPDATE {outboxMessageTable}
            SET locked_until = @lockedUntil
            WHERE id IN @ids; 
            """,
            new
            {
                ids = messages.Select(x => x.Id),
                lockedUntil
            });
    }

    public async Task ProcessOutboxMessageAsync(OutboxMessage message, string outboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {outboxMessageTable}
            SET processed_at = NOW()
            WHERE id = @outboxMessageId;
            """,
            new
            {
                outboxMessageId = message.Id,
            });
    }

    public async Task ProcessOutboxMessagesAsync(IReadOnlyCollection<OutboxMessage> messages, string outboxMessageTable)
    {
        await connection.ExecuteAsync(
            $"""
            UPDATE {outboxMessageTable}
            SET processed_at = NOW()
            WHERE id IN @outboxMessageIds;
            """,
            new
            {
                outboxMessageIds = messages.Select(x => x.Id)
            });
    }
}