using Osirisgate.CleanArchitectureCore.Attributes.Type;
using UsecaseBase = Osirisgate.CleanArchitectureCore.Usecase.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

/// <summary>
/// Specific stub for testing conflicts with multiple implementations.
/// Without environment attributes to always be scanned.
/// </summary>
[Usecase(AsInterface = typeof(IMultipleUsecaseInterface))]
public sealed class ConflictTestFirstUsecase : UsecaseBase, IMultipleUsecaseInterface
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PresentResponse(CleanArchitectureCore.Response.Response.Create(true, CleanArchitectureCore.Response.StatusCode.Ok, "success", null));
        return Task.CompletedTask;
    }

    public string GetName() => "ConflictTestFirst";
}

