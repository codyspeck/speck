namespace Speck.DurableMessaging;

internal interface IPipeline
{
    Task SendAsync(object message);
}
