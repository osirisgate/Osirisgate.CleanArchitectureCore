using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when a method call is invalid for the object's current state.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class InvalidOperationException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;

    /// <summary>
    /// Creates an <see cref="InvalidOperationException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="InvalidOperationException"/> instance.</returns>
    public static InvalidOperationException Create(string message, IDictionary<string, object>? details = null)
    {
        return new InvalidOperationException(new Dictionary<string, object>
        {
            ["message"] = message,
            ["details"] = details ?? new Dictionary<string, object>()
        });
    }

    /// <summary>
    /// Creates an <see cref="InvalidOperationException"/> with the specified error dictionary.
    /// </summary>
    /// <param name="errors">A dictionary containing error details.</param>
    /// <returns>A new <see cref="InvalidOperationException"/> instance.</returns>
    public static InvalidOperationException Create(IDictionary<string, object> errors)
    {
        return new InvalidOperationException(errors);
    }
}
