using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Mailbox;

internal class MailboxPollingService(
    IServiceProvider services,
    MailboxConfiguration configuration,
    MailboxSignals signals,
    ILogger<MailboxPollingService>? logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var numberOfMessagesPolled = await RunLoopAsync(configuration, services);

                if (numberOfMessagesPolled > 0)
                    continue;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger?.LogError(exception, "An unexpected error occurred while polling the mailbox.");
            }

            await Task.WhenAny(
                signals.Get(configuration.Table),
                Task.Delay(configuration.IdlePollingInterval, stoppingToken));
            
            signals.Reset(configuration.Table);
        }
    }

    private static async Task<int> RunLoopAsync(MailboxConfiguration configuration, IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IMailboxMessageRepository>();
        var messageSerializer = scope.ServiceProvider.GetRequiredService<MessageSerializer>();
        var mailboxMessageTypes = scope.ServiceProvider.GetRequiredService<MailboxMessageTypeCollection>();
        
        IReadOnlyCollection<MailboxMessage> mailboxMessages = ReadOnlyCollection<MailboxMessage>.Empty;
            
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            mailboxMessages = await repository.GetMailboxMessagesAsync(configuration.Table, configuration.PollSize);

            await repository.LockMailboxMessagesAsync(
                mailboxMessages,
                configuration.Table,
                DateTime.UtcNow.Add(MailboxDefaults.MessageLockDuration));
        });
            
        foreach (var mailboxMessage in mailboxMessages)
        {
            var message = messageSerializer.Deserialize(
                mailboxMessage.Content,
                mailboxMessageTypes.Get(mailboxMessage.Type));
                
            await scope.ServiceProvider
                .GetRequiredKeyedService<IMailboxMessagePipeline>(mailboxMessage.Type)
                .SendAsync(new MailboxMessageContext(mailboxMessage.Id, configuration.Table, message));
        }

        return mailboxMessages.Count;
    }
}