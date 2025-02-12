using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Speck.DurableMessaging.MySql;

namespace Speck.DurableMessaging.IntegrationTests.Inbox;

public class InboxTests(MySqlFixture mySqlFixture, TestInboxRepository repository) : TestBase
{
    [Test]
    public async Task Inserts_inbox_message()
    {
        await using var services = new ServiceCollection()
            .AddScoped(_ => mySqlFixture.CreateConnection())
            .AddDurableMessaging(messaging => messaging
                .UseMySql())
            .BuildServiceProvider();

        await using var scope = services.CreateAsyncScope();
        
        var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();

        await inbox.InsertAsync(new object());

        await Should.NotThrowAsync(repository.GetFirstInboxMessageAsync());
    }
}