using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Response;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Usecase;

/// <summary>
/// An abstract base class for use cases that provides core functionality for handling requests and presenters.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public abstract class Usecase : IUsecase
{
    private IRequest? _request;
    private IPresenter? _presenter;

    /// <inheritdoc/>
    public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public IUsecase WithRequest(IRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        _request = request;
        return this;
    }

    /// <inheritdoc/>
    public IUsecase WithPresenter(IPresenter presenter)
    {
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));

        _presenter = presenter;
        return this;
    }

    /// <inheritdoc/>
    public IRequest? GetRequest()
    {
        return _request;
    }

    /// <inheritdoc/>
    public IPresenter? GetPresenter()
    {
        return _presenter;
    }

    /// <summary>
    /// Passes the response object to the attached presenter.
    /// </summary>
    /// <param name="response">The response to present.</param>
    /// <exception cref="ArgumentNullException">Thrown if the response is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the presenter is not set.</exception>
    protected void PresentResponse(IResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));

        if (_presenter == null)
            throw InvalidOperationException.Create("Presenter is not set. Call WithPresenter(presenter) first.");

        _presenter.Present(response);
    }

    /// <summary>
    /// Gets a specific field from the attached request's payload.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <param name="defaultValue">An optional default value if the field is not found.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the request has not been set.</exception>
    protected T? GetField<T>(string fieldPath, object? defaultValue = null)
    {
        if (_request == null)
            throw InvalidOperationException.Create("Request is not set. Call WithRequest(request) first.");

        return _request.GetField<T>(fieldPath, defaultValue);
    }

    /// <summary>
    /// Gets a specific field from the attached request's modified payload.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <param name="defaultValue">An optional default value if the field is not found.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the request has not been set.</exception>
    protected T? GetModifiedField<T>(string fieldPath, object? defaultValue = null)
    {
        if (_request == null)
            throw InvalidOperationException.Create("Request is not set. Call WithRequest(request) first.");

        return _request.GetModifiedField<T>(fieldPath, defaultValue);
    }

    /// <summary>
    /// Gets the entire payload from the attached request.
    /// </summary>
    /// <returns>A dictionary representing the request payload.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the request has not been set.</exception>
    protected IReadOnlyDictionary<string, object> GetPayload()
    {
        if (_request == null)
            throw InvalidOperationException.Create("Request is not set. Call WithRequest(request) first.");

        return _request.GetPayload();
    }

    /// <summary>
    /// Gets the entire modified payload from the attached request.
    /// </summary>
    /// <returns>A dictionary representing the request modified payload.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the request has not been set.</exception>
    protected IDictionary<string, object> GetModifiedPayload()
    {
        if (_request == null)
            throw InvalidOperationException.Create("Request is not set. Call WithRequest(request) first.");

        return _request.GetModifiedPayload();
    }
}
