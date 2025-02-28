namespace Speck.DurableMessaging.Inbox;

internal interface IInboxMessagePipeline
{
    Task SendAsync(InboxMessageContext context);
}
