using Dapper;

namespace Speck.DurableMessaging.IntegrationTests.Inbox;

public class TestInboxRepository(MySqlFixture mySqlFixture)
{
    public async Task<InboxMessage> GetFirstInboxMessageAsync()
    {
        await using var connection = mySqlFixture.CreateConnection();

        return await connection.QueryFirstAsync<InboxMessage>("SELECT * FROM inbox_message LIMIT 1;");
    }
}