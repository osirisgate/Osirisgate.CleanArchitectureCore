using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[Service(AsInterface = typeof(ITestServiceWithImplementation), Implementation = typeof(TestServiceImplementation))]
public sealed class TestServiceImplementation : ITestServiceWithImplementation
{
    public string GetValue() => "test-implementation";
}

