using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Pipeline;
using Osirisgate.CleanArchitectureCore.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Pipeline;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[WhenDev]
[WhenTest]
[Pipeline(AsInterface = typeof(IMultiEnvironmentPipeline))]
public sealed class MultiEnvironmentPipeline : IMultiEnvironmentPipeline
{
    public IPipeline WithPreMiddlewares(IReadOnlyList<IPreMiddleware>? preMiddlewares)
    {
        return this;
    }

    public IPipeline WithPostMiddlewares(IReadOnlyList<IPostMiddleware>? postMiddlewares)
    {
        return this;
    }

    public Task ExecuteAsync(IUsecase useCase, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

