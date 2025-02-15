using Microsoft.Extensions.DependencyInjection;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageRouter(IServiceProvider services)
{
    public async Task SendAsync(object message)
    {
    }
}