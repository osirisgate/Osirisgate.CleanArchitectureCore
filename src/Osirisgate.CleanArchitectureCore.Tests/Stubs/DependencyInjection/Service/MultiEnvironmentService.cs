using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IMultiEnvironmentService
{
    public string GetValue();
}

[WhenDev]
[WhenTest]
[Service(AsInterface = typeof(IMultiEnvironmentService))]
public sealed class MultiEnvironmentService : IMultiEnvironmentService
{
    public string GetValue() => "multi-env";
}

