using Osirisgate.CleanArchitectureCore.Attributes.Type;
using UsecaseBase = Osirisgate.CleanArchitectureCore.Usecase.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
/// <summary>
/// This class specifies an interface in the attribute but does not implement it.
/// Used to test the path if (!componentImplementsInterface) in ValidateInterfaceHasImplementation.
/// </summary>
[Usecase(AsInterface = typeof(IUsecaseWithoutImplementation))]
public sealed class UsecaseWithoutInterfaceImplementation : UsecaseBase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public string GetValue() => "test";
}

