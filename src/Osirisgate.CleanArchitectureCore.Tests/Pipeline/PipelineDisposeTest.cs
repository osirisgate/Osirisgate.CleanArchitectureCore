using Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;
using ObjectDisposedException = Osirisgate.CleanArchitectureCore.Exception.ObjectDisposedException;
using PipelineClass = Osirisgate.CleanArchitectureCore.Pipeline.Pipeline;

namespace Osirisgate.CleanArchitectureCore.Tests.Pipeline;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class PipelineDisposeTest
{
    [Fact]
    public void TestDisposeAfterDisposeThrowsObjectDisposedException()
    {
        var pipeline = new PipelineClass();
        pipeline.Dispose();

        Assert.Throws<ObjectDisposedException>(() => pipeline.WithPreMiddlewares([]));
        Assert.Throws<ObjectDisposedException>(() => pipeline.WithPostMiddlewares([]));
    }

    [Fact]
    public void TestDisposeCanBeCalledMultipleTimesDoesNotThrow()
    {
        var pipeline = new PipelineClass();

        pipeline.Dispose();
        pipeline.Dispose();
        pipeline.Dispose();
    }

    [Fact]
    public async Task TestExecuteAsyncAfterDisposeThrowsObjectDisposedException()
    {
        var pipeline = new PipelineClass();
        var usecase = new CustomUsecase();
        pipeline.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => pipeline.ExecuteAsync(usecase));
    }
}
