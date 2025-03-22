using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Speck.DataflowExtensions;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.Mailbox;

internal class MailboxMessageBatchPipeline<TMessage> : IMailboxMessagePipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<MailboxMessageContext> _pipeline;
    private readonly IServiceProvider _services;

    public MailboxMessageBatchPipeline(IServiceProvider services, MailboxMessageBatchHandlerConfiguration configuration)
    {
        _pipeline = DataflowPipelineBuilder.Create<MailboxMessageContext>()
            .Batch(configuration.BatchSize, configuration.BatchTimeout)
            .Build(HandleMailboxMessageAsync);
        
        _services = services;
    }
    
    public async Task SendAsync(MailboxMessageContext context)
    {
        await _pipeline.SendAsync(context);
    }

    private async Task HandleMailboxMessageAsync(MailboxMessageContext[] contexts)
    {
        await using var scope = _services.CreateAsyncScope();

        try
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var repository = scope.ServiceProvider.GetRequiredService<IMailboxMessageRepository>();
            var handler = scope.ServiceProvider.GetRequiredService<IMailboxMessageBatchHandler<TMessage>>();

            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var mailboxMessageTables = contexts
                    .Select(c => c.MailboxMessageTable)
                    .Distinct()
                    .ToArray();

                if (mailboxMessageTables.Length > 1)
                    throw new InvalidOperationException(
                        "Cannot process an mailbox batch with multiple mailbox message tables.");

                var mailboxMessages = await repository.GetMailboxMessagesAsync(
                    contexts.Select(c => c.MailboxMessageId),
                    mailboxMessageTables[0]);

                await handler.HandleAsync(contexts
                    .Select(c => (TMessage)c.Message)
                    .ToArray());

                await repository.ProcessMailboxMessagesAsync(mailboxMessages, mailboxMessageTables[0]);
            });
        }
        catch (Exception exception)
        {
            scope.ServiceProvider
                .GetService<ILogger<MailboxMessageBatchPipeline<TMessage>>>()?
                .LogError(
                    exception,
                    "Failed to process a mailbox message batch with messages {MailboxMessageIds}.",
                    contexts.Select(c => c.MailboxMessageId));
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}