using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.DurableMessaging.Inbox;

internal class InboxMessagePipeline<TMessage> : IPipeline, IAsyncDisposable
{
    private readonly DataflowPipeline<TMessage> _pipeline;
    private readonly IServiceProvider _services;

    public InboxMessagePipeline(IServiceProvider services, InboxMessageHandlerConfiguration configuration)
    {
        _pipeline = new DataflowPipelineBuilder<TMessage>()
            .Build(HandleInboxMessageAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = configuration.BoundedCapacity,
                MaxDegreeOfParallelism = configuration.MaxDegreeOfParallelism
            });
        
        _services = services;
    }
    
    public async Task SendAsync(object message)
    {
        await _pipeline.SendAsync((TMessage)message);
    }

    private async Task HandleInboxMessageAsync(TMessage message)
    {
        await using var scope = _services.CreateAsyncScope();
        
        await scope.ServiceProvider
            .GetRequiredService<IInboxMessageHandler<TMessage>>()
            .HandleAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}