using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;
using ObjectDisposedException = Osirisgate.CleanArchitectureCore.Exception.ObjectDisposedException;
using PipelineClass = Osirisgate.CleanArchitectureCore.Pipeline.Pipeline;

namespace Osirisgate.CleanArchitectureCore.Tests.DependencyInjection;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class MiddlewareResolverDisposeTest
{
    [Fact]
    public void TestDisposeAfterDisposeThrowsObjectDisposedException()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(provider);
        resolver.Dispose();

        Assert.Throws<ObjectDisposedException>(() => resolver.GetPreMiddlewares(typeof(object)));
        Assert.Throws<ObjectDisposedException>(() => resolver.GetPostMiddlewares(typeof(object)));
    }

    [Fact]
    public void TestDisposeCanBeCalledMultipleTimesDoesNotThrow()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(provider);

        resolver.Dispose();
        resolver.Dispose();
        resolver.Dispose();
    }

    [Fact]
    public void TestConfigurePipelineAfterDisposeThrowsObjectDisposedException()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(provider);
        var pipeline = new PipelineClass();
        resolver.Dispose();

        Assert.Throws<ObjectDisposedException>(() => resolver.ConfigurePipeline(pipeline, typeof(object)));
    }
}
