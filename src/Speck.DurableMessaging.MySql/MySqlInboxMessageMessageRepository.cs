using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging.MySql;

public class MySqlInboxMessageRepository(MySqlConnection connection) : IInboxMessageRepository
{
    public async Task<IReadOnlyCollection<InboxMessage>> GetInboxMessagesAsync(string inboxMessageTable, int count)
    {
        var inboxMessages = await connection.QueryAsync<InboxMessage>(
            $"SELECT id FROM {inboxMessageTable} LIMIT @count;",
            new { count });

        return inboxMessages.ToArray();
    }
}