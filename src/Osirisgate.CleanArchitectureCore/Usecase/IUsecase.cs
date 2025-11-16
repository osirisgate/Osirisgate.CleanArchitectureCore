using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Usecase;

/// <summary>
/// Defines the contract for a use case, which encapsulates application-specific business logic.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IUsecase
{
    /// <summary>
    /// Executes the business logic of the use case.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ExecuteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a request object to the use case.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <returns>The use case instance for fluent chaining.</returns>
    public IUsecase WithRequest(IRequest request);

    /// <summary>
    /// Gets the request object attached to the use case.
    /// </summary>
    /// <returns>The request object.</returns>
    public IRequest? GetRequest();

    /// <summary>
    /// Attaches a presenter to the use case.
    /// </summary>
    /// <param name="presenter">The presenter object.</param>
    /// <returns>The use case instance for fluent chaining.</returns>
    public IUsecase WithPresenter(IPresenter presenter);

    /// <summary>
    /// Gets the presenter attached to the use case.
    /// </summary>
    /// <returns>The presenter object.</returns>
    public IPresenter? GetPresenter();
}
