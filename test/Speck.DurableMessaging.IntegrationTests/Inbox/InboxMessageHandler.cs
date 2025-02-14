using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging.IntegrationTests.Inbox;

public class InboxMessageHandler : IInboxMessageHandler<InboxMessage>
{
    public Task HandleAsync(InboxMessage message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}
