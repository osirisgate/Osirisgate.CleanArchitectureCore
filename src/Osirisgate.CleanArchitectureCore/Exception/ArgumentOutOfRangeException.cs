using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ArgumentOutOfRangeException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;
}
