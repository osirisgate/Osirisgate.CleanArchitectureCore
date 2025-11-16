using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Request;

/// <summary>
/// Extension methods for <see cref="IRequest"/> to improve API ergonomics.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class RequestExtensions
{
    /// <summary>
    /// Gets a required field from the request payload, throwing an exception if the field is missing or null.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to. Must be a non-nullable type.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the field is missing or null.</exception>
    public static T GetRequired<T>(this IRequest request, string fieldPath) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return request.GetField<T>(fieldPath) ?? throw InvalidOperationException.Create($"Required field '{fieldPath}' is missing or null.");
    }

    /// <summary>
    /// Attempts to get a field from the request payload.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <param name="value">When this method returns, contains the field value if found; otherwise, the default value for the type.</param>
    /// <returns><c>true</c> if the field was found and successfully cast; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public static bool TryGetField<T>(this IRequest request, string fieldPath, out T? value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        value = request.GetField<T>(fieldPath);
        return value != null;
    }

    /// <summary>
    /// Gets a field from the request payload, throwing an exception if the field is missing or null.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the field is missing or null.</exception>
    public static T GetOrThrow<T>(this IRequest request, string fieldPath)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return request.GetField<T>(fieldPath) ?? throw InvalidOperationException.Create($"Field '{fieldPath}' is missing or null.");
    }

    /// <summary>
    /// Gets a required field from the modified payload, throwing an exception if the field is missing or null.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to. Must be a non-nullable type.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the field is missing or null.</exception>
    public static T GetRequiredModified<T>(this IRequest request, string fieldPath) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return request.GetModifiedField<T>(fieldPath) ?? throw InvalidOperationException.Create($"Required modified field '{fieldPath}' is missing or null.");
    }

    /// <summary>
    /// Attempts to get a field from the modified payload.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation.</param>
    /// <param name="value">When this method returns, contains the field value if found; otherwise, the default value for the type.</param>
    /// <returns><c>true</c> if the field was found and successfully cast; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public static bool TryGetModifiedField<T>(this IRequest request, string fieldPath, out T? value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        value = request.GetModifiedField<T>(fieldPath);
        return value != null;
    }
}
