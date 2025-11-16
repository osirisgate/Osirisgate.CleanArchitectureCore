using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Pipeline;

namespace Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;

/// <summary>
/// Defines the contract for resolving and combining global and usecase-specific middlewares for pipeline execution.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IMiddlewareResolver : IDisposable
{
    /// <summary>
    /// Gets the pre-middlewares for a specific use case, combining global and usecase-specific middlewares.
    /// </summary>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The combined list of pre-middlewares.</returns>
    public IReadOnlyList<IPreMiddleware> GetPreMiddlewares(Type usecaseType);

    /// <summary>
    /// Gets the post-middlewares for a specific use case, combining global and usecase-specific middlewares.
    /// </summary>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The combined list of post-middlewares.</returns>
    public IReadOnlyList<IPostMiddleware> GetPostMiddlewares(Type usecaseType);

    /// <summary>
    /// Configures a pipeline with middlewares for a specific use case.
    /// </summary>
    /// <param name="pipeline">The pipeline to configure.</param>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The configured pipeline.</returns>
    public IPipeline ConfigurePipeline(IPipeline pipeline, Type usecaseType);
}
