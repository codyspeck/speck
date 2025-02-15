using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            
            var repository = scope.ServiceProvider.GetRequiredService<IInboxMessageRepository>();

            var inboxMessages = await repository.GetInboxMessagesAsync(configuration.Table, configuration.PollSize);

            foreach (var inboxMessage in inboxMessages)
            {
                var message = messageSerializer.Deserialize(
                    inboxMessage.Content,
                    inboxMessageTypes.Get(inboxMessage.Type));
                
                await scope.ServiceProvider
                    .GetRequiredKeyedService<IPipeline>(inboxMessage.Type)
                    .SendAsync(message);
            }
            
            await Task.Delay(configuration.IdlePollingInterval, stoppingToken);
        }
    }
}