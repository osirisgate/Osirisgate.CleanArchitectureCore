using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.DependencyInjection;
using Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.ThreadSafety;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ThreadSafetyTest
{
    [Fact]
    public async Task TestMiddlewareResolverGetPreMiddlewaresIsThreadSafe()
    {
        var services = new ServiceCollection();
        services.AddCleanArchitectureCore();
        var provider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(provider);
        var usecaseType = typeof(TestUsecase);

        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => resolver.GetPreMiddlewares(usecaseType)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var first = results[0];
        Assert.All(results, r => Assert.Equal(first.Count, r.Count));
    }

    [Fact]
    public async Task TestMiddlewareResolverGetPostMiddlewaresIsThreadSafe()
    {
        var services = new ServiceCollection();
        services.AddCleanArchitectureCore();
        var provider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(provider);
        var usecaseType = typeof(TestUsecase);

        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => resolver.GetPostMiddlewares(usecaseType)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var first = results[0];
        Assert.All(results, r => Assert.Equal(first.Count, r.Count));
    }

    [Fact]
    public async Task TestRequestGetFieldIsThreadSafe()
    {
        var payload = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = "value2",
            ["nested"] = new Dictionary<string, object> { ["key"] = "value" }
        };
        var request = new CustomRequest(payload);

        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() =>
            {
                var key = i % 2 == 0 ? "key1" : "key2";
                return request.GetField<string>(key);
            }))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.NotNull(r));
        Assert.All(results, r => Assert.True(r == "value1" || r == "value2"));
    }

    [Fact]
    public async Task TestRequestGetModifiedFieldIsThreadSafe()
    {
        var payload = new Dictionary<string, object> { ["key"] = "required" };
        var request = new CustomRequestWithPayload(payload);
        request.ModifiedPayload(new Dictionary<string, object> { ["modified"] = "value" });

        var tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => request.GetModifiedField<string>("modified")))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal("value", r));
    }

    [Fact]
    public async Task TestRequestToDtoIsThreadSafe()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "test@example.com",
            ["name"] = "Test"
        };
        var request = new CustomRequest(payload);

        var tasks = Enumerable.Range(0, 50)
            .Select(_ => Task.Run(() => request.GetField<string>("email")))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal("test@example.com", r));
    }

    private class TestUsecase : CleanArchitectureCore.Usecase.Usecase
    {
        public override Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

