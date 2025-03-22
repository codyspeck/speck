using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Mailbox;

internal class MailboxMessagePipeline<TMessage> : IMailboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<MailboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public MailboxMessagePipeline(IServiceProvider services, MailboxMessageHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<MailboxMessageContext>()
            .Build(HandleMailboxMessageAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = configuration.BoundedCapacity,
                MaxDegreeOfParallelism = configuration.MaxDegreeOfParallelism
            });
        
        _services = services;
    }
    
    public async Task SendAsync(MailboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleMailboxMessageAsync(MailboxMessageContext context)
    {
        await using var scope = _services.CreateAsyncScope();

        await scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>()
            .ExecuteInTransactionAsync(async () =>
            {
                var mailboxMessage = await scope.ServiceProvider
                    .GetRequiredService<IMailboxMessageRepository>()
                    .GetMailboxMessageAsync(context.MailboxMessageId, context.MailboxMessageTable);

                if (mailboxMessage.ProcessedAt is not null)
                    return;
                
                await scope.ServiceProvider
                    .GetRequiredService<IMailboxMessageHandler<TMessage>>()
                    .HandleAsync((TMessage)context.Message);
                
                await scope.ServiceProvider
                    .GetRequiredService<IMailboxMessageRepository>()
                    .ProcessMailboxMessageAsync(mailboxMessage, context.MailboxMessageTable);
            });
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}