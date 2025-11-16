using Osirisgate.CleanArchitectureCore.Response;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Presenter;

/// <summary>
/// An abstract base class for presenters that provides default functionality for handling and formatting a <see cref="IResponse"/>.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public abstract class Presenter : IPresenter
{
    private IResponse? _response;

    /// <inheritdoc/>
    public void Present(IResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        _response = response;
    }

    /// <inheritdoc/>
    public IResponse? GetResponse()
    {
        return _response;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetFormattedResponse()
    {
        if (_response == null)
            throw InvalidOperationException.Create("Response is not set. Call Present(response) first.");

        return _response.GetOutput();
    }
}
