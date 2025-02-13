using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Speck.DurableMessaging.Inbox;

internal class InboxPollingService(IServiceProvider services, InboxConfiguration configuration) : BackgroundService
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
                Console.WriteLine($"Queried inbox message: {inboxMessage.Id}.");
            }
            
            await Task.Delay(configuration.IdlePollingInterval, stoppingToken);
        }
    }
}