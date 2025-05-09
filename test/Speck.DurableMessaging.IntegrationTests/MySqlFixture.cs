﻿using Dapper;
using MySqlConnector;
using Testcontainers.MySql;

namespace Speck.DurableMessaging.IntegrationTests;

public sealed class MySqlFixture : IAsyncDisposable
{
    private readonly MySqlContainer _container = new MySqlBuilder()
        .Build();

    public MySqlConnection CreateConnection()
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder(_container.GetConnectionString())
        {
            IgnoreCommandTransaction = true
        };
        
        return new MySqlConnection(connectionStringBuilder.ToString());
    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        await using var connection = CreateConnection();

        await connection.ExecuteAsync(
            """
            CREATE TABLE mailbox_messages (
                id           CHAR(36)     NOT NULL PRIMARY KEY,
                type         VARCHAR(100) NOT NULL,
                content      JSON         NOT NULL,
                message_key  VARCHAR(100) NULL,
                created_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
                locked_until DATETIME     NULL,
                processed_at DATETIME     NULL
            );
            
            CREATE TABLE inbox_messages_2 (
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