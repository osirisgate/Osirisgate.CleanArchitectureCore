using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface ITestUsecaseWithInterface { }

[Usecase(AsInterface = typeof(ITestUsecaseWithInterface))]
public sealed class TestUsecaseWithInterface : CleanArchitectureCore.Usecase.Usecase, ITestUsecaseWithInterface
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
