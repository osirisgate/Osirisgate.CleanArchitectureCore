using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestProdRepository
{
    public string GetValue();
}

[WhenTest]
[WhenProd]
[Repository(AsInterface = typeof(ITestProdRepository))]
public sealed class TestProdRepository : ITestProdRepository
{
    public string GetValue() => "test-prod-repo";
}

