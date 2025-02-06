using System.Threading.Tasks.Dataflow;

using AutoFixture;

using Shouldly;

namespace Speck.DataflowExtensions.UnitTests;

public class DataflowPipelineTests
{
    public class Observer<T>
    {
        private readonly List<T> _items = [];

        public IReadOnlyCollection<T> Items => _items;
        
        public void Add(T item)
        {
            _items.Add(item);
        }

        public Task AddAsync(T item)
        {
            _items.Add(item);
            return Task.CompletedTask;
        }
    }

    private readonly Fixture _fixture = new();
    private readonly Observer<string> _observer = new();

    [Test]
    public async Task Disposing_pipeline_ignores_operation_canceled_exceptions()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Build(_ => { }, new ExecutionDataflowBlockOptions { CancellationToken = cancellationTokenSource.Token });

        await cancellationTokenSource.CancelAsync();
        
        await Should.NotThrowAsync(pipeline.DisposeAsync().AsTask());
    }
    
    [Test]
    public async Task Single_stage_pipeline_performs_action()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Build(_observer.Add);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }
    
    [Test]
    public async Task Single_stage_pipeline_performs_async_action()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Build(_observer.AddAsync);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }

    [Test]
    public async Task Two_stage_pipeline_performs_transformation()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Select(_ => expected)
            .Build(_observer.Add);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }
    
    [Test]
    public async Task Two_stage_pipeline_performs_async_transformation()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Select(_ => Task.FromResult(expected))
            .Build(_observer.AddAsync);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }

    [Test]
    public async Task Three_stage_pipeline_performs_transformation()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Select(value => value)
            .Select(_ => expected)
            .Build(_observer.Add);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }
    
    [Test]
    public async Task Three_stage_pipeline_performs_async_transformation()
    {
        var expected = _fixture.Create<string>();
        
        var pipeline = new DataflowPipelineBuilder<string>()
            .Select(value => value)
            .Select(_ => Task.FromResult(expected))
            .Build(_observer.AddAsync);
        
        await pipeline.SendAsync(expected);
        await pipeline.DisposeAsync();
        
        _observer.Items.ShouldContain(expected);
    }
}
