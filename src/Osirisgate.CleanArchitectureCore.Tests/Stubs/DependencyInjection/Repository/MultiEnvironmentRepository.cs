using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IMultiEnvironmentRepository
{
    public string GetValue();
}

[WhenDev]
[WhenTest]
[Repository(AsInterface = typeof(IMultiEnvironmentRepository))]
public sealed class MultiEnvironmentRepository : IMultiEnvironmentRepository
{
    public string GetValue() => "multi-env-repo";
}

