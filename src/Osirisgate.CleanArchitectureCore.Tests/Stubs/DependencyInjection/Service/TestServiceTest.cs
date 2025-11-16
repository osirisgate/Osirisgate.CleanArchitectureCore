using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestServiceTest
{
    public string GetValue();
}

[WhenTest]
[Service(AsInterface = typeof(ITestServiceTest))]
public sealed class TestServiceTest : ITestServiceTest
{
    public string GetValue() => "test";
}
