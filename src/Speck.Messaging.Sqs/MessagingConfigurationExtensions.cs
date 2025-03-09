namespace Speck.Messaging.Sqs;

public static class MessagingConfigurationExtensions
{
    /// <summary>
    /// Adds support for sending to and consuming from AWS SQS queues.
    /// </summary>
    /// <param name="messagingConfiguration">The messaging configuration.</param>
    /// <param name="configure">Configures the messaging SQS configuration.</param>
    /// <returns>This.</returns>
    public static MessagingConfiguration AddSqs(
        this MessagingConfiguration messagingConfiguration,
        Action<MessagingSqsConfiguration> configure)
    {
        var messagingSqsConfiguration = new MessagingSqsConfiguration();
        
        configure(messagingSqsConfiguration);
        
        return messagingConfiguration;
    }
}
