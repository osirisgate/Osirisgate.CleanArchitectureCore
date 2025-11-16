using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[Usecase]
public sealed class TestUsecase : CleanArchitectureCore.Usecase.Usecase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
