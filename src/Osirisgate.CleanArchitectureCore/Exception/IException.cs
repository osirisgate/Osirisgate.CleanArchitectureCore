namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Defines the contract for custom exceptions within the Clean Architecture Core library.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IException
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    /// <returns>An integer representing the HTTP status code.</returns>
    public int GetErrorCode();

    /// <summary>
    /// Gets the primary error message.
    /// </summary>
    /// <returns>The primary error message string.</returns>
    public string GetMessage();

    /// <summary>
    /// Gets a specific error message from within the 'details' dictionary, if available.
    /// </summary>
    /// <returns>A specific detail message string.</returns>
    public string GetDetailsMessage();

    /// <summary>
    /// Formats the entire exception into a standardized dictionary for API responses.
    /// </summary>
    /// <returns>A dictionary containing the formatted error response.</returns>
    public IDictionary<string, object> Format();

    /// <summary>
    /// Gets the original dictionary of errors passed to the exception's constructor.
    /// </summary>
    /// <returns>The original error dictionary.</returns>
    public IDictionary<string, object> GetErrors();

    /// <summary>
    /// Gets the 'details' dictionary from the error payload.
    /// </summary>
    /// <returns>A dictionary containing detailed error information.</returns>
    public IDictionary<string, object> GetDetails();
}
