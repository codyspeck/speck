using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Outbox;

internal class OutboxMessagePipeline<TMessage> : IOutboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<OutboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public OutboxMessagePipeline(IServiceProvider services, OutboxMessageHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<OutboxMessageContext>()
            .Build(HandleOutboxMessageAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = configuration.BoundedCapacity,
                MaxDegreeOfParallelism = configuration.MaxDegreeOfParallelism
            });
        
        _services = services;
    }
    
    public async Task SendAsync(OutboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleOutboxMessageAsync(OutboxMessageContext context)
    {
        await using var scope = _services.CreateAsyncScope();

        await scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>()
            .ExecuteInTransactionAsync(async () =>
            {
                var outboxMessage = await scope.ServiceProvider
                    .GetRequiredService<IOutboxMessageRepository>()
                    .GetOutboxMessageAsync(context.OutboxMessageId, context.OutboxMessageTable);

                if (outboxMessage.ProcessedAt is not null)
                    return;
                
                await scope.ServiceProvider
                    .GetRequiredService<IOutboxMessageHandler<TMessage>>()
                    .HandleAsync((TMessage)context.Message);
                
                await scope.ServiceProvider
                    .GetRequiredService<IOutboxMessageRepository>()
                    .ProcessOutboxMessageAsync(outboxMessage, context.OutboxMessageTable);
            });
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}