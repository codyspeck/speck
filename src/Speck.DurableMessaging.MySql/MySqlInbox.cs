using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging.MySql;

internal class MySqlInbox(MySqlConnection connection, InboxMessageFactory factory) : IInbox
{
    public async Task InsertAsync(object message)
    {
        var inboxMessage = factory.Create(message);
        
        await connection.ExecuteAsync(
            """
            INSERT INTO inbox_message (id, type, content, created_at)
            VALUES (@id, @type, @content, @created_at);
            """,
            new
            {
                id = inboxMessage.Id,
                type = inboxMessage.Type,
                content = inboxMessage.Content,
                created_at = inboxMessage.CreatedAt
            });
    }
}
