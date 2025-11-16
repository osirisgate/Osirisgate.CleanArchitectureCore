using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when an explicit conversion fails at runtime.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class InvalidCastException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;

    /// <summary>
    /// Creates an <see cref="InvalidCastException"/> with the specified source and target types.
    /// </summary>
    /// <param name="sourceTypeName">The name of the source type.</param>
    /// <param name="targetTypeName">The name of the target type.</param>
    /// <param name="value">The value that could not be cast (optional).</param>
    /// <returns>A new <see cref="InvalidCastException"/> instance.</returns>
    public static InvalidCastException Create(string sourceTypeName, string targetTypeName, object? value = null)
    {
        var valueString = value?.ToString() ?? "null";
        return new InvalidCastException(new Dictionary<string, object>
        {
            ["message"] = $"Cannot cast {sourceTypeName} to {targetTypeName}. Value: {valueString}",
            ["details"] = new Dictionary<string, object>
            {
                ["source_type"] = sourceTypeName,
                ["target_type"] = targetTypeName,
                ["value"] = valueString
            }
        });
    }

    /// <summary>
    /// Creates an <see cref="InvalidCastException"/> with the specified error dictionary.
    /// </summary>
    /// <param name="errors">A dictionary containing error details.</param>
    /// <returns>A new <see cref="InvalidCastException"/> instance.</returns>
    public static InvalidCastException Create(IDictionary<string, object> errors)
    {
        return new InvalidCastException(errors);
    }
}
