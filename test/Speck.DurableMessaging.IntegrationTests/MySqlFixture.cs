using Dapper;
using MySqlConnector;
using Testcontainers.MySql;

namespace Speck.DurableMessaging.IntegrationTests;

public sealed class MySqlFixture : IAsyncDisposable
{
    private readonly MySqlContainer _container = new MySqlBuilder()
        .Build();

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_container.GetConnectionString());
    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        await using var connection = CreateConnection();

        await connection.ExecuteAsync(
            """
            CREATE TABLE inbox_messages (
                id           CHAR(36)     NOT NULL PRIMARY KEY,
                type         VARCHAR(100) NOT NULL,
                content      JSON         NOT NULL,
                message_key  VARCHAR(100) NULL,
                created_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
                locked_until DATETIME     NULL,
                processed_at DATETIME     NULL
            );
            """);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}