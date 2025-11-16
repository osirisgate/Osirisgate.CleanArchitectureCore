using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[WhenTest]
[Service(AsInterface = typeof(ITestServiceWithoutImplementation))]
public sealed class TestServiceWithInterfaceWithoutImplementation : ITestServiceWithoutImplementation
{
    public string GetValue() => "test";
}
