using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Pipeline;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Pipeline;

[WhenDev]
[Pipeline(AsInterface = typeof(IMultiplePipelineInterface))]
public sealed class FirstPipeline : IMultiplePipelineInterface
{
    public string GetName() => "First";

    public IPipeline WithPreMiddlewares(System.Collections.Generic.IReadOnlyList<Osirisgate.CleanArchitectureCore.Middleware.IPreMiddleware>? preMiddlewares)
    {
        return this;
    }

    public IPipeline WithPostMiddlewares(System.Collections.Generic.IReadOnlyList<Osirisgate.CleanArchitectureCore.Middleware.IPostMiddleware>? postMiddlewares)
    {
        return this;
    }

    public Task ExecuteAsync(Osirisgate.CleanArchitectureCore.Usecase.IUsecase useCase, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

