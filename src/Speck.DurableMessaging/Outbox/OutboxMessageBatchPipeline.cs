using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessageBatchPipeline<TMessage> : IOutboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<OutboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public OutboxMessageBatchPipeline(IServiceProvider services, OutboxMessageBatchHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<OutboxMessageContext>()
            .Batch(configuration.BatchSize, configuration.BatchTimeout)
            .Build(HandleOutboxMessageAsync);
        
        _services = services;
    }
    
    public async Task SendAsync(OutboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleOutboxMessageAsync(OutboxMessageContext[] contexts)
    {
        await using var scope = _services.CreateAsyncScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var handler = scope.ServiceProvider.GetRequiredService<IOutboxMessageBatchHandler<TMessage>>();

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var outboxMessageTables = contexts
                .Select(c => c.OutboxMessageTable)
                .Distinct()
                .ToArray();

            if (outboxMessageTables.Length > 1)
                throw new InvalidOperationException("Cannot process an Outbox batch with multiple Outbox message tables.");
            
            var outboxMessages = await repository.GetOutboxMessagesAsync(
                contexts.Select(c => c.OutboxMessageId),
                outboxMessageTables[0]);
            
            await handler.HandleAsync(contexts
                .Select(c => (TMessage)c.Message)
                .ToArray());

            await repository.ProcessOutboxMessagesAsync(outboxMessages, outboxMessageTables[0]);
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}