using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestServiceSingleton
{
    public int GetInstanceId();
}

[Service(AsInterface = typeof(ITestServiceSingleton), Lifetime = ServiceLifetime.Singleton)]
public sealed class TestServiceSingleton : ITestServiceSingleton
{
    private static int _instanceCount = 0;
    private readonly int _instanceId;

    public TestServiceSingleton()
    {
        _instanceId = Interlocked.Increment(ref _instanceCount);
    }

    public int GetInstanceId() => _instanceId;
}
