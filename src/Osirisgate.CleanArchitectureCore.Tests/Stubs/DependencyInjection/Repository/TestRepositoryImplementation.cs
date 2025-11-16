using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[Repository(AsInterface = typeof(ITestRepositoryWithImplementation), Implementation = typeof(TestRepositoryImplementation))]
public sealed class TestRepositoryImplementation : ITestRepositoryWithImplementation
{
    public string GetValue() => "repository-implementation";
}

