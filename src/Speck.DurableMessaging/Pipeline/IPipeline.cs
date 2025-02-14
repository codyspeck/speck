namespace Speck.DurableMessaging.Pipeline;

internal interface IPipeline<in TMessage>
{
    Task SendAsync(TMessage message, CancellationToken cancellationToken);
}
