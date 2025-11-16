using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Pipeline;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Pipeline;

/// <summary>
/// Specific stub for testing conflicts with multiple implementations.
/// Without environment attributes to always be scanned.
/// </summary>
[Pipeline(AsInterface = typeof(IMultiplePipelineInterface))]
public sealed class ConflictTestSecondPipeline : IMultiplePipelineInterface
{
    public string GetName() => "ConflictTestSecond";

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

