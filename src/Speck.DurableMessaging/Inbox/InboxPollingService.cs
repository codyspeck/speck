using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Inbox;

internal class InboxPollingService(
    IServiceProvider services,
    InboxConfiguration configuration,
    InboxMessageTypeCollection inboxMessageTypes,
    MessageSerializer messageSerializer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = services.CreateAsyncScope();

            var inboxMessages = Enumerable.Empty<InboxMessage>();
            
            await scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>()
                .ExecuteInTransactionAsync(async () =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IInboxMessageRepository>();

                    inboxMessages = await repository.GetInboxMessagesAsync(configuration.Table, configuration.PollSize);

                    await repository.LockInboxMessagesAsync(
                        inboxMessages,
                        configuration.Table,
                        DateTime.UtcNow.AddMinutes(5));
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
            
            await Task.Delay(configuration.IdlePollingInterval, stoppingToken);
        }
    }
}