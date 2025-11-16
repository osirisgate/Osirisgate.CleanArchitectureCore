using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception for malformed or invalid request content.
/// This exception defaults to a 400 Bad Request status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class BadRequestContentException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.BadRequest;
}
