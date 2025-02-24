using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging;

internal interface IPipeline
{
    Task SendAsync(InboxMessageContext context);
}
