using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when a null argument is passed to a method that does not accept it as a valid argument.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ArgumentNullException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the argument is null.
    /// </summary>
    /// <param name="argument">The argument to check for null.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
    public static void ThrowIfNull(object? argument, string? paramName = null)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(new Dictionary<string, object>
            {
                ["message"] = $"Argument '{paramName ?? "value"}' cannot be null.",
                ["details"] = new Dictionary<string, object?>
                {
                    ["parameter"] = paramName
                }
            });
        }
    }
}
