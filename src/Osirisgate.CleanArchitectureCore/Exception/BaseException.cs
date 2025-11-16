using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// The base class for all custom exceptions in the Clean Architecture Core library.
/// It provides a structured way to handle errors with consistent output.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public abstract class BaseException : System.Exception, IException
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception. Defaults to BadRequest (400).
    /// </summary>
    protected virtual StatusCode ErrorCode { get; } = StatusCode.BadRequest;
    private readonly IDictionary<string, object> _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseException"/> class with a dictionary of error details.
    /// </summary>
    /// <param name="errors">A dictionary containing error details. Expected keys are 'message' and 'details'.</param>
    protected BaseException(IDictionary<string, object> errors)
        : base(errors.TryGetValue("message", out var msg) ? msg?.ToString() ?? string.Empty : string.Empty)
    {
        _errors = errors ?? new Dictionary<string, object>();
    }

    /// <inheritdoc/>
    public IDictionary<string, object> Format()
    {
        return new Dictionary<string, object>
        {
            ["status"] = Status.Error.GetValue(),
            ["error_code"] = ErrorCode.GetValue(),
            ["message"] = GetMessage(),
            ["details"] = GetDetails(),
        };
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetErrors()
    {
        return _errors;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetDetails()
    {
        return _errors.TryGetValue("details", out var details) && details is IDictionary<string, object> dict
            ? dict
            : new Dictionary<string, object>();
    }

    /// <inheritdoc/>
    public string GetDetailsMessage()
    {
        if (GetDetails().TryGetValue("error", out var error) && error is string errorMessage)
        {
            return errorMessage;
        }
        return string.Empty;
    }

    /// <inheritdoc/>
    public string GetMessage()
    {
        return Message;
    }

    /// <inheritdoc/>
    public int GetErrorCode()
    {
        return ErrorCode.GetValue();
    }
}
