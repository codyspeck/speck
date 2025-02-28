using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Outbox;

internal class OutboxPollingService(
    IServiceProvider services,
    OutboxConfiguration configuration,
    OutboxSignals signals,
    ILogger<OutboxPollingService>? logger) : BackgroundService
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
                logger?.LogError(exception, "An unexpected error occurred while polling the Outbox.");
            }

            await Task.WhenAny(
                signals.Get(configuration.Table),
                Task.Delay(configuration.IdlePollingInterval, stoppingToken));
            
            signals.Reset(configuration.Table);
        }
    }

    private static async Task<int> RunLoopAsync(OutboxConfiguration configuration, IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var messageSerializer = scope.ServiceProvider.GetRequiredService<MessageSerializer>();
        var outboxMessageTypes = scope.ServiceProvider.GetRequiredService<OutboxMessageTypeCollection>();
        
        IReadOnlyCollection<OutboxMessage> outboxMessages = ReadOnlyCollection<OutboxMessage>.Empty;
            
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            outboxMessages = await repository.GetOutboxMessagesAsync(configuration.Table, configuration.PollSize);

            await repository.LockOutboxMessagesAsync(
                outboxMessages,
                configuration.Table,
                DateTime.UtcNow.Add(OutboxDefaults.MessageLockDuration));
        });
            
        foreach (var outboxMessage in outboxMessages)
        {
            var message = messageSerializer.Deserialize(
                outboxMessage.Content,
                outboxMessageTypes.Get(outboxMessage.Type));
                
            await scope.ServiceProvider
                .GetRequiredKeyedService<IOutboxMessagePipeline>(outboxMessage.Type)
                .SendAsync(new OutboxMessageContext(outboxMessage.Id, configuration.Table, message));
        }

        return outboxMessages.Count;
    }
}