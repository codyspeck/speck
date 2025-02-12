using Dapper;

using MySqlConnector;

namespace Speck.DurableMessaging.MySql;

internal class MySqlInbox(MySqlConnection connection) : IInbox
{
    public async Task InsertAsync(object message)
    {
        await connection.ExecuteAsync(
            "INSERT INTO inbox_message (id) VALUES (@id);",
            new { id = Guid.CreateVersion7() });
    }
}
