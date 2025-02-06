using System.Threading.Tasks.Dataflow;

namespace Speck.DataflowExtensions;

file static class DataflowPipelineBuilder
{
    public static DataflowLinkOptions DataflowLinkOptions { get; } = new() { PropagateCompletion = true };
    
    public static ExecutionDataflowBlockOptions ExecutionDataflowBlockOptions { get; } = new();
}

public class DataflowPipelineBuilder<TInput>
{
    public DataflowPipeline<TInput> Build(Action<TInput> action)
    {
        return Build(action, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Action<TInput> action, ExecutionDataflowBlockOptions options)
    {
        var inputBlock = new ActionBlock<TInput>(action, options);

        return new DataflowPipeline<TInput>(inputBlock, inputBlock);
    }
    
    public DataflowPipeline<TInput> Build(Func<TInput, Task> func)
    {
        return Build(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Func<TInput, Task> func, ExecutionDataflowBlockOptions options)
    {
        var inputBlock = new ActionBlock<TInput>(func, options);

        return new DataflowPipeline<TInput>(inputBlock, inputBlock);
    }

    public DataflowPipelineBuilder<TInput, TOutput> Select<TOutput>(Func<TInput, TOutput> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput> Select<TOutput>(
        Func<TInput, TOutput> selector,
        ExecutionDataflowBlockOptions options)
    {
        return new DataflowPipelineBuilder<TInput, TOutput>(new TransformBlock<TInput, TOutput>(selector, options));
    }
    
    public DataflowPipelineBuilder<TInput, TOutput> Select<TOutput>(Func<TInput, Task<TOutput>> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput> Select<TOutput>(
        Func<TInput, Task<TOutput>> selector,
        ExecutionDataflowBlockOptions options)
    {
        return new DataflowPipelineBuilder<TInput, TOutput>(new TransformBlock<TInput, TOutput>(selector, options));
    }
}

public class DataflowPipelineBuilder<TInput, TOutput>
{
    private readonly IPropagatorBlock<TInput, TOutput> _inputBlock;
    
    internal DataflowPipelineBuilder(IPropagatorBlock<TInput, TOutput> inputBlock)
    {
        _inputBlock = inputBlock;
    }
    
    public DataflowPipeline<TInput> Build(Action<TOutput> action)
    {
        return Build(action, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Action<TOutput> action, ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new ActionBlock<TOutput>(action, options);
        
        _inputBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipeline<TInput>(_inputBlock, terminalBlock);
    }
    
    public DataflowPipeline<TInput> Build(Func<TOutput, Task> func)
    {
        return Build(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Func<TOutput, Task> func, ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new ActionBlock<TOutput>(func, options);
        
        _inputBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipeline<TInput>(_inputBlock, terminalBlock);
    }

    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(Func<TOutput, TNext> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(
        Func<TOutput, TNext> func,
        ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new TransformBlock<TOutput, TNext>(func, options);
        
        _inputBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipelineBuilder<TInput, TOutput, TNext>(_inputBlock, terminalBlock);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(Func<TOutput, Task<TNext>> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(
        Func<TOutput, Task<TNext>> func,
        ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new TransformBlock<TOutput, TNext>(func, options);
        
        _inputBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipelineBuilder<TInput, TOutput, TNext>(_inputBlock, terminalBlock);
    }
}

public class DataflowPipelineBuilder<TInput, TPrevious, TOutput>
{
    private readonly ITargetBlock<TInput> _inputBlock;
    private readonly IPropagatorBlock<TPrevious, TOutput> _terminalBlock;
    
    internal DataflowPipelineBuilder(
        ITargetBlock<TInput> inputBlock,
        IPropagatorBlock<TPrevious, TOutput> terminalBlock)
    {
        _inputBlock = inputBlock;
        _terminalBlock = terminalBlock;
    }
    
    public DataflowPipeline<TInput> Build(Action<TOutput> action)
    {
        return Build(action, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Action<TOutput> action, ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new ActionBlock<TOutput>(action, options);
        
        _terminalBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipeline<TInput>(_inputBlock, terminalBlock);
    }
    
    public DataflowPipeline<TInput> Build(Func<TOutput, Task> func)
    {
        return Build(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipeline<TInput> Build(Func<TOutput, Task> func, ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new ActionBlock<TOutput>(func, options);
        
        _terminalBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipeline<TInput>(_inputBlock, terminalBlock);
    }

    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(Func<TOutput, TNext> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(
        Func<TOutput, TNext> func,
        ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new TransformBlock<TOutput, TNext>(func, options);
        
        _terminalBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipelineBuilder<TInput, TOutput, TNext>(_inputBlock, terminalBlock);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(Func<TOutput, Task<TNext>> func)
    {
        return Select(func, DataflowPipelineBuilder.ExecutionDataflowBlockOptions);
    }
    
    public DataflowPipelineBuilder<TInput, TOutput, TNext> Select<TNext>(
        Func<TOutput, Task<TNext>> func,
        ExecutionDataflowBlockOptions options)
    {
        var terminalBlock = new TransformBlock<TOutput, TNext>(func, options);
        
        _terminalBlock.LinkTo(terminalBlock, DataflowPipelineBuilder.DataflowLinkOptions);
        
        return new DataflowPipelineBuilder<TInput, TOutput, TNext>(_inputBlock, terminalBlock);
    }
}
