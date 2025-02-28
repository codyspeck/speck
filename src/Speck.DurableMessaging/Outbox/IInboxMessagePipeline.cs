namespace Speck.DurableMessaging.Outbox;

internal interface IOutboxMessagePipeline
{
    Task SendAsync(OutboxMessageContext context);
}
