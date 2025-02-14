using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging;

public class DurableMessagingConfiguration
{
    public IServiceCollection Services { get; }
    
    internal InboxMessageRegistry InboxMessageRegistry { get; } = new();

    internal List<InboxConfiguration> InboxConfigurations { get; } = [];

    internal DurableMessagingConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Adds an inbox. Multiple inboxes can be configured if each one has a uniquely configured inbox message table.
    /// </summary>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInbox()
    {
        return AddInbox(_ => { });
    }
    
    /// <summary>
    /// Adds an inbox. Multiple inboxes can be configured if each one has a uniquely configured inbox message table.
    /// </summary>
    /// <param name="configure">Configures the inbox.</param>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInbox(Action<InboxConfiguration> configure)
    {
        var configuration = new InboxConfiguration();
        configure(configuration);
        InboxConfigurations.Add(configuration);
        return this;
    }
    
    /// <summary>
    /// Configures an <see cref="IInboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the inbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <typeparam name="THandler">The type of the inbox message handler to configure.</typeparam>
    /// <typeparam name="TMessage">The type of the inbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInboxMessageHandler<THandler, TMessage>(string messageType)
        where THandler : class, IInboxMessageHandler<TMessage>
    {
        return AddInboxMessageHandler<THandler, TMessage>(messageType, _ => { }); 
    }
    
    /// <summary>
    /// Configures an <see cref="IInboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the inbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <param name="configure">Configures the inbox message handler.</param>
    /// <typeparam name="THandler">The type of the inbox message handler to configure.</typeparam>
    /// <typeparam name="TMessage">The type of the inbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInboxMessageHandler<THandler, TMessage>(
        string messageType,
        Action<InboxMessageHandlerConfiguration> configure)
        where THandler : class, IInboxMessageHandler<TMessage>
    {
        var configuration = new InboxMessageHandlerConfiguration();
        configure(configuration);
        InboxMessageRegistry.Add<TMessage>(messageType);
        Services.AddTransient<IInboxMessageHandler<TMessage>, THandler>();
        Services.AddSingleton(BuildInboxMessageDataflowPipeline<TMessage>);
        return this;
    }

    private static DataflowPipeline<TMessage> BuildInboxMessageDataflowPipeline<TMessage>(IServiceProvider services)
    {
        return new DataflowPipelineBuilder<TMessage>()
            .Build(async message =>
            {
                await using var scope = services.CreateAsyncScope();

                await scope.ServiceProvider
                    .GetRequiredService<IInboxMessageHandler<TMessage>>()
                    .HandleAsync(message);
            });
    }
}
