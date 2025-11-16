using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestRepository
{
    public string GetValue();
}

[Repository]
public sealed class TestRepository : ITestRepository
{
    public string GetValue() => "repository";
}
