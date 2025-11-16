using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestServiceTransient
{
    public int GetInstanceId();
}

[Service(AsInterface = typeof(ITestServiceTransient), Lifetime = ServiceLifetime.Transient)]
public sealed class TestServiceTransient : ITestServiceTransient
{
    private static int _instanceCount = 0;
    private readonly int _instanceId;

    public TestServiceTransient()
    {
        _instanceId = Interlocked.Increment(ref _instanceCount);
    }

    public int GetInstanceId() => _instanceId;
}
