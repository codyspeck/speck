﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Testing;
using Speck.DurableMessaging.Inbox;
using Speck.DurableMessaging.MySql;

namespace Speck.DurableMessaging.IntegrationTests.Inbox;

public class InboxTests(MySqlFixture mySqlFixture, TestInboxRepository repository) : TestBase
{
    [Test]
    [Experimental("EXTEXP0016")]
    public async Task Inserts_inbox_message()
    {
        using var host = FakeHost
            .CreateBuilder()
            .ConfigureServices((_, services) => services
                .AddScoped(_ => mySqlFixture.CreateConnection())
                .AddDurableMessaging(messaging => messaging
                    .AddInbox()
                    .AddInboxMessageHandler<InboxMessageHandler, InboxMessage>("inbox-message", handler => handler
                        .WithBoundedCapacity(100)
                        .WithMaxDegreeOfParallelism(1))
                    .UseMySql()))
            .Build();
        
        await host.StartAsync();
        
        await using var scope = host.Services.CreateAsyncScope();
        
        var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();

        await inbox.InsertAsync(new InboxMessage { Id = Guid.NewGuid() });
        await inbox.InsertAsync(new InboxMessage { Id = Guid.NewGuid() });
        await inbox.InsertAsync(new InboxMessage { Id = Guid.NewGuid() });

        await Task.Delay(TimeSpan.FromSeconds(6));
    }
}