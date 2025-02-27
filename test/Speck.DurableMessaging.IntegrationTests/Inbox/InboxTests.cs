using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Testing;
using MySqlConnector;
using Polly;
using Polly.Retry;
using Shouldly;
using Speck.DurableMessaging.Inbox;
using Speck.DurableMessaging.MySql;

namespace Speck.DurableMessaging.IntegrationTests.Inbox;

public class InboxTests(MySqlFixture mySqlFixture, Fixture fixture) : TestBase
{
    /// <summary>
    /// Retrying handles variations in latency when inserting messages into the inbox, waiting for them to be polled,
    /// and then waiting for them to be executed. 
    /// </summary>
    private static readonly ResiliencePipeline RetryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions())
        .Build();
    
    [Test]
    [Experimental("EXTEXP0016")]
    public async Task Inbox_message_is_handled()
    {
        var observer = new TestObserver<TestInboxMessage>();

        var testInboxMessage = fixture.Create<TestInboxMessage>();
        
        using var host = FakeHost
            .CreateBuilder()
            .ConfigureServices((_, services) => services
                .AddScoped(_ => mySqlFixture.CreateConnection())
                .AddSingleton(observer)
                .AddDurableMessaging(messaging => messaging
                    .AddInbox()
                    .AddInboxMessageHandler<TestInboxMessageHandler, TestInboxMessage>("test-inbox-message")
                    .UseMySql()))
            .Build();
        
        await host.StartAsync();
        
        await using (var scope = host.Services.CreateAsyncScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<IInbox>()
                .InsertAsync(testInboxMessage);
        }

        RetryPipeline.Execute(() => observer.Items.ShouldContain(x => x == testInboxMessage));
    }
    
    [Test]
    [Experimental("EXTEXP0016")]
    public async Task Inbox_message_batch_is_handled()
    {
        var observer = new TestObserver<TestInboxMessage[]>();

        var testInboxMessages = fixture
            .CreateMany<TestInboxMessage>()
            .ToArray();
        
        using var host = FakeHost
            .CreateBuilder()
            .ConfigureServices((_, services) => services
                .AddScoped(_ => mySqlFixture.CreateConnection())
                .AddSingleton(observer)
                .AddDurableMessaging(messaging => messaging
                    .AddInbox(inbox => inbox
                        .WithTable("inbox_messages_2")
                        .WithPollSize(testInboxMessages.Length))
                    .AddInboxMessageBatchHandler<TestInboxMessageBatchHandler, TestInboxMessage>(
                        "test-inbox-message",
                        handler => handler.WithBatchSize(testInboxMessages.Length))
                    .UseMySql()))
            .Build();
        
        await host.StartAsync();
        
        await using (var scope = host.Services.CreateAsyncScope())
        {
            var connection = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
            
            await connection.OpenAsync();
            
            await using var transaction = await connection.BeginTransactionAsync();

            var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();

            foreach (var message in testInboxMessages)
                await inbox.InsertAsync(message);

            await transaction.CommitAsync();
        }

        RetryPipeline.Execute(() => observer.Items.First().ShouldBe(testInboxMessages));
    }

    private record TestInboxMessage(Guid Id);

    private class TestInboxMessageHandler(TestObserver<TestInboxMessage> observer) : IInboxMessageHandler<TestInboxMessage>
    {
        public Task HandleAsync(TestInboxMessage message)
        {
            observer.Add(message);
            return Task.CompletedTask;
        }
    }
    
    private class TestInboxMessageBatchHandler(TestObserver<TestInboxMessage[]> observer)
        : IInboxMessageBatchHandler<TestInboxMessage>
    {
        public Task HandleAsync(IReadOnlyCollection<TestInboxMessage> messages)
        {
            observer.Add(messages.ToArray());
            return Task.CompletedTask;
        }
    }
}