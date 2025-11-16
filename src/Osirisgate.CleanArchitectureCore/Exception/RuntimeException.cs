using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents a generic runtime exception.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class RuntimeException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;
}
