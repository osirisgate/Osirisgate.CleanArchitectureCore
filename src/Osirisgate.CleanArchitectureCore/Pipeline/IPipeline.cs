using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Usecase;

namespace Osirisgate.CleanArchitectureCore.Pipeline;

/// <summary>
/// Defines the contract for a pipeline that orchestrates middleware and use case execution.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IPipeline
{
    /// <summary>
    /// Sets the pre middlewares.
    /// </summary>
    /// <param name="preMiddlewares">The list of pre middlewares to execute before the use case.</param>
    /// <returns>The pipeline instance for fluent chaining.</returns>
    public IPipeline WithPreMiddlewares(IReadOnlyList<IPreMiddleware>? preMiddlewares);

    /// <summary>
    /// Sets the post middlewares.
    /// </summary>
    /// <param name="postMiddlewares">The list of post middlewares to execute after the use case.</param>
    /// <returns>The pipeline instance for fluent chaining.</returns>
    public IPipeline WithPostMiddlewares(IReadOnlyList<IPostMiddleware>? postMiddlewares);

    /// <summary>
    /// Executes the pipeline with the specified use case.
    /// </summary>
    /// <param name="useCase">The use case to execute.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecuteAsync(IUsecase useCase, CancellationToken cancellationToken = default);
}
