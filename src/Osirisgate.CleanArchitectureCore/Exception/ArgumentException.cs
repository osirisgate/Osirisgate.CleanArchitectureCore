using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when one of the arguments provided to a method is not valid.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ArgumentException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;

    /// <summary>
    /// Creates an <see cref="ArgumentException"/> with the specified message and parameter name.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <returns>A new <see cref="ArgumentException"/> instance.</returns>
    public static ArgumentException Create(string message, string? paramName = null)
    {
        return new ArgumentException(new Dictionary<string, object>
        {
            ["message"] = message,
            ["details"] = new Dictionary<string, object?>
            {
                ["parameter"] = paramName,
                ["reason"] = "The argument provided is not valid."
            }
        });
    }

    /// <summary>
    /// Creates an <see cref="ArgumentException"/> with the specified error dictionary.
    /// </summary>
    /// <param name="errors">A dictionary containing error details.</param>
    /// <returns>A new <see cref="ArgumentException"/> instance.</returns>
    public static ArgumentException Create(IDictionary<string, object> errors)
    {
        return new ArgumentException(errors);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the string is null or empty.
    /// </summary>
    /// <param name="argument">The string argument to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="ArgumentException">Thrown if the argument is null or empty.</exception>
    public static void ThrowIfNullOrEmpty(string? argument, string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw Create($"Argument '{paramName ?? "value"}' cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the string is null, empty, or whitespace.
    /// </summary>
    /// <param name="argument">The string argument to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="ArgumentException">Thrown if the argument is null, empty, or whitespace.</exception>
    public static void ThrowIfNullOrWhiteSpace(string? argument, string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw Create($"Argument '{paramName ?? "value"}' cannot be null, empty, or whitespace.", paramName);
        }
    }
}
