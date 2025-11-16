using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestServiceDev
{
    public string GetValue();
}

[WhenDev]
[Service(AsInterface = typeof(ITestServiceDev))]
public sealed class TestServiceDev : ITestServiceDev
{
    public string GetValue() => "dev";
}
