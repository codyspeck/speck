using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Inbox;

internal class InboxPollingService(
    IServiceProvider services,
    InboxConfiguration configuration,
    InboxSignals signals,
    ILogger<InboxPollingService>? logger) : BackgroundService
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
                logger?.LogError(exception, "An unexpected error occurred while polling the inbox.");
            }

            await Task.WhenAny(
                signals.Get(configuration.Table),
                Task.Delay(configuration.IdlePollingInterval, stoppingToken));
            
            signals.Reset(configuration.Table);
        }
    }

    private static async Task<int> RunLoopAsync(InboxConfiguration configuration, IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IInboxMessageRepository>();
        var messageSerializer = scope.ServiceProvider.GetRequiredService<MessageSerializer>();
        var inboxMessageTypes = scope.ServiceProvider.GetRequiredService<InboxMessageTypeCollection>();
        
        IReadOnlyCollection<InboxMessage> inboxMessages = ReadOnlyCollection<InboxMessage>.Empty;
            
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            inboxMessages = await repository.GetInboxMessagesAsync(configuration.Table, configuration.PollSize);

            await repository.LockInboxMessagesAsync(
                inboxMessages,
                configuration.Table,
                DateTime.UtcNow.Add(InboxDefaults.MessageLockDuration));
        });
            
        foreach (var inboxMessage in inboxMessages)
        {
            var message = messageSerializer.Deserialize(
                inboxMessage.Content,
                inboxMessageTypes.Get(inboxMessage.Type));
                
            await scope.ServiceProvider
                .GetRequiredKeyedService<IPipeline>(inboxMessage.Type)
                .SendAsync(new InboxMessageContext(inboxMessage.Id, configuration.Table, message));
        }

        return inboxMessages.Count;
    }
}