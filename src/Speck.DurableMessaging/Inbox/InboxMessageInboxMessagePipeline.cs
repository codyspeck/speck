using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessageInboxMessagePipeline<TMessage> : IInboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<InboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public InboxMessageInboxMessagePipeline(IServiceProvider services, InboxMessageHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<InboxMessageContext>()
            .Build(HandleInboxMessageAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = configuration.BoundedCapacity,
                MaxDegreeOfParallelism = configuration.MaxDegreeOfParallelism
            });
        
        _services = services;
    }
    
    public async Task SendAsync(InboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleInboxMessageAsync(InboxMessageContext context)
    {
        await using var scope = _services.CreateAsyncScope();

        await scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>()
            .ExecuteInTransactionAsync(async () =>
            {
                var inboxMessage = await scope.ServiceProvider
                    .GetRequiredService<IInboxMessageRepository>()
                    .GetInboxMessageAsync(context.InboxMessageId, context.InboxMessageTable);

                if (inboxMessage.ProcessedAt is not null)
                    return;
                
                await scope.ServiceProvider
                    .GetRequiredService<IInboxMessageHandler<TMessage>>()
                    .HandleAsync((TMessage)context.Message);
                
                await scope.ServiceProvider
                    .GetRequiredService<IInboxMessageRepository>()
                    .ProcessInboxMessageAsync(inboxMessage, context.InboxMessageTable);
            });
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}