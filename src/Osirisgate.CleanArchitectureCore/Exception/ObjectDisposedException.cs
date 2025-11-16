using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Exception;

/// <summary>
/// Represents an exception that is thrown when an operation is performed on a disposed object.
/// This exception defaults to a 500 Internal Server Error status code.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ObjectDisposedException(IDictionary<string, object> errors) : BaseException(errors)
{
    /// <inheritdoc/>
    protected override StatusCode ErrorCode { get; } = StatusCode.InternalServerError;

    /// <summary>
    /// Creates an <see cref="ObjectDisposedException"/> with the specified object name.
    /// </summary>
    /// <param name="objectName">The name of the disposed object.</param>
    /// <returns>A new <see cref="ObjectDisposedException"/> instance.</returns>
    public static ObjectDisposedException Create(string objectName)
    {
        return new ObjectDisposedException(new Dictionary<string, object>
        {
            ["message"] = $"Cannot access a disposed object. Object name: '{objectName}'.",
            ["details"] = new Dictionary<string, object>
            {
                ["object_name"] = objectName,
                ["reason"] = "The object has been disposed and is no longer available for use."
            }
        });
    }

    /// <summary>
    /// Creates an <see cref="ObjectDisposedException"/> with the specified error dictionary.
    /// </summary>
    /// <param name="errors">A dictionary containing error details.</param>
    /// <returns>A new <see cref="ObjectDisposedException"/> instance.</returns>
    public static ObjectDisposedException Create(IDictionary<string, object> errors)
    {
        return new ObjectDisposedException(errors);
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the object is disposed.
    /// </summary>
    /// <param name="isDisposed">A value indicating whether the object is disposed.</param>
    /// <param name="objectName">The name of the object being checked.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the object is disposed.</exception>
    public static void ThrowIf(bool isDisposed, string objectName)
    {
        if (isDisposed)
        {
            throw Create(objectName);
        }
    }
}
