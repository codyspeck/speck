using Dapper;
using MySqlConnector;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging.MySql;

internal class MySqlInbox(MySqlConnection connection) : IInbox
{
    public async Task InsertAsync<TMessage>(TMessage message)
    {
        await connection.ExecuteAsync(
            "INSERT INTO inbox_message (id) VALUES (@id);",
            new { id = Guid.CreateVersion7() });
    }
}
