using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Presenter;

/// <summary>
/// Defines the contract for a Presenter, which is responsible for formatting a use case's <see cref="IResponse"/>.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IPresenter
{
    /// <summary>
    /// Receives the response from a use case to be formatted.
    /// </summary>
    /// <param name="response">The response object from the use case.</param>
    public void Present(IResponse response);

    /// <summary>
    /// Gets the raw, unformatted response object.
    /// </summary>
    /// <returns>The <see cref="IResponse"/> instance.</returns>
    public IResponse? GetResponse();

    /// <summary>
    /// Gets the response formatted into a standardized dictionary.
    /// </summary>
    /// <returns>A dictionary representing the final, formatted response.</returns>
    public IDictionary<string, object> GetFormattedResponse();
}
