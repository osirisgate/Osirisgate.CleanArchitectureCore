using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;

[WhenTest]
[Repository(AsInterface = typeof(IMultipleRepositoryInterface))]
public sealed class SecondRepository : IMultipleRepositoryInterface
{
    public string GetName() => "Second";
}

