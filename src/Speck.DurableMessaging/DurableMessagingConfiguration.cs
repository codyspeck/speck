using Microsoft.Extensions.DependencyInjection;

namespace Speck.DurableMessaging;

public class DurableMessagingConfiguration
{
    public IServiceCollection Services { get; }

    internal DurableMessagingConfiguration(IServiceCollection services)
    {
        Services = services;
    }
    
    /// <summary>
    /// Configures an <see cref="IInboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <typeparam name="THandler">The type of the inbox message handler to configure.</typeparam>
    /// <typeparam name="TMessage">The type of the inbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInboxMessageHandler<THandler, TMessage>()
        where THandler : class, IInboxMessageHandler<TMessage>
    {
        Services.AddTransient<IInboxMessageHandler<TMessage>, THandler>();
        
        return this;
    }
}
