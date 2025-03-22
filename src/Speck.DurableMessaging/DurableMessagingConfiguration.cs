using Microsoft.Extensions.DependencyInjection;
using Speck.DurableMessaging.Mailbox;

namespace Speck.DurableMessaging;

public class DurableMessagingConfiguration
{
    public IServiceCollection Services { get; }
    
    internal MailboxMessageTypeCollection MailboxMessageTypeCollection { get; } = new();

    internal List<MailboxConfiguration> MailboxConfigurations { get; } = [];

    internal DurableMessagingConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Adds an mailbox. Multiple mailboxes can be configured if each one has a uniquely configured mailbox message table.
    /// </summary>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailbox()
    {
        return AddMailbox(_ => { });
    }
    
    /// <summary>
    /// Adds an mailbox. Multiple mailboxes can be configured if each one has a uniquely configured mailbox message table.
    /// </summary>
    /// <param name="configure">Configures the mailbox.</param>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailbox(Action<MailboxConfiguration> configure)
    {
        var configuration = new MailboxConfiguration();
        configure(configuration);
        MailboxConfigurations.Add(configuration);
        return this;
    }
    
    /// <summary>
    /// Configures an <see cref="IMailboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the mailbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <typeparam name="THandler">The type of the mailbox message handler to configure.</typeparam>
    /// <typeparam name="TMessage">The type of the mailbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailboxMessageHandler<THandler, TMessage>(string messageType)
        where THandler : class, IMailboxMessageHandler<TMessage>
    {
        return AddMailboxMessageHandler<THandler, TMessage>(messageType, _ => { }); 
    }
    
    /// <summary>
    /// Configures an <see cref="IMailboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the mailbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <param name="configure">Configures the mailbox message handler.</param>
    /// <typeparam name="THandler">The type of the mailbox message handler.</typeparam>
    /// <typeparam name="TMessage">The type of the mailbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailboxMessageHandler<THandler, TMessage>(
        string messageType,
        Action<MailboxMessageHandlerConfiguration> configure)
        where THandler : class, IMailboxMessageHandler<TMessage>
    {
        var configuration = new MailboxMessageHandlerConfiguration();
        configure(configuration);
        MailboxMessageTypeCollection.Add<TMessage>(messageType);
        Services
            .AddTransient<IMailboxMessageHandler<TMessage>, THandler>()
            .AddKeyedSingleton<IMailboxMessagePipeline>(messageType, (services, _) =>
                new MailboxMessageMailboxMessagePipeline<TMessage>(services, configuration));
        return this;
    }
    
    /// <summary>
    /// Configures an <see cref="IMailboxMessageBatchHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the mailbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <typeparam name="THandler">The type of the mailbox message batch handler.</typeparam>
    /// <typeparam name="TMessage">The type of the mailbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailboxMessageBatchHandler<THandler, TMessage>(string messageType)
        where THandler : class, IMailboxMessageBatchHandler<TMessage>
    {
        return AddMailboxMessageBatchHandler<THandler, TMessage>(messageType, _ => { });
    }
    
    /// <summary>
    /// Configures an <see cref="IMailboxMessageBatchHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <param name="messageType">
    /// The string representation of the message type. This is stored in the mailbox message table and correlated back to 
    /// <see cref="TMessage"/> in the deserialization process.
    /// </param>
    /// <param name="configure">Configures the mailbox message batch handler.</param>
    /// <typeparam name="THandler">The type of the mailbox message batch handler.</typeparam>
    /// <typeparam name="TMessage">The type of the mailbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddMailboxMessageBatchHandler<THandler, TMessage>(
        string messageType,
        Action<MailboxMessageBatchHandlerConfiguration> configure)
        where THandler : class, IMailboxMessageBatchHandler<TMessage>
    {
        var configuration = new MailboxMessageBatchHandlerConfiguration();
        configure(configuration);
        MailboxMessageTypeCollection.Add<TMessage>(messageType);
        Services
            .AddTransient<IMailboxMessageBatchHandler<TMessage>, THandler>()
            .AddKeyedSingleton<IMailboxMessagePipeline>(messageType, (services, _) =>
                new MailboxMessageBatchMailboxMessagePipeline<TMessage>(services, configuration));
        return this;
    }
}
