using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using UsecaseBase = Osirisgate.CleanArchitectureCore.Usecase.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

[WhenDev]
[Usecase(AsInterface = typeof(IMultipleUsecaseInterface))]
public sealed class FirstUsecase : UsecaseBase, IMultipleUsecaseInterface
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PresentResponse(CleanArchitectureCore.Response.Response.Create(true, CleanArchitectureCore.Response.StatusCode.Ok, "success", null));
        return Task.CompletedTask;
    }

    public string GetName() => "First";
}

