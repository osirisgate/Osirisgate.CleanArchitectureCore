using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestServiceWithInterface
{
    public string GetValue();
}

[Service(AsInterface = typeof(ITestServiceWithInterface))]
public sealed class TestServiceWithInterface : ITestServiceWithInterface
{
    public string GetValue() => "test-interface";
}
