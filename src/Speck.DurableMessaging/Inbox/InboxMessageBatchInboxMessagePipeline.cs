using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageBatchInboxMessagePipeline<TMessage> : IInboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<InboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public InboxMessageBatchInboxMessagePipeline(IServiceProvider services, InboxMessageBatchHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<InboxMessageContext>()
            .Batch(configuration.BatchSize, configuration.BatchTimeout)
            .Build(HandleInboxMessageAsync);
        
        _services = services;
    }
    
    public async Task SendAsync(InboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleInboxMessageAsync(InboxMessageContext[] contexts)
    {
        await using var scope = _services.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IInboxMessageRepository>();
        var handler = scope.ServiceProvider.GetRequiredService<IInboxMessageBatchHandler<TMessage>>();

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var inboxMessageTables = contexts
                .Select(c => c.InboxMessageTable)
                .Distinct()
                .ToArray();

            if (inboxMessageTables.Length > 1)
                throw new InvalidOperationException("Cannot process an inbox batch with multiple inbox message tables.");
            
            var inboxMessages = await repository.GetInboxMessagesAsync(
                contexts.Select(c => c.InboxMessageId),
                inboxMessageTables[0]);
            
            await handler.HandleAsync(contexts
                .Select(c => (TMessage)c.Message)
                .ToArray());

            await repository.ProcessInboxMessagesAsync(inboxMessages, inboxMessageTables[0]);
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}