using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Response;

/// <summary>
/// Extension methods for <see cref="IResponse"/> to improve API ergonomics.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class ResponseExtensions
{
    /// <summary>
    /// Gets a required field from the response data, throwing an exception if the field is missing or null.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to. Must be a non-nullable type.</typeparam>
    /// <param name="response">The response instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the field is missing or null.</exception>
    public static T GetRequired<T>(this IResponse response, string fieldPath) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return response.Get<T>(fieldPath) ?? throw InvalidOperationException.Create($"Required field '{fieldPath}' is missing or null.");
    }

    /// <summary>
    /// Attempts to get a field from the response data.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="response">The response instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <param name="value">When this method returns, contains the field value if found; otherwise, the default value for the type.</param>
    /// <returns><c>true</c> if the field was found and successfully cast; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public static bool TryGet<T>(this IResponse response, string fieldPath, out T? value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        value = response.Get<T>(fieldPath);
        return value != null;
    }

    /// <summary>
    /// Gets a field from the response data, throwing an exception if the field is missing or null.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="response">The response instance.</param>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <returns>The field's value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the field is missing or null.</exception>
    public static T GetOrThrow<T>(this IResponse response, string fieldPath)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return response.Get<T>(fieldPath) ?? throw InvalidOperationException.Create($"Field '{fieldPath}' is missing or null.");
    }

    /// <summary>
    /// Sets a field in the response data if it doesn't already exist.
    /// </summary>
    /// <param name="response">The response instance.</param>
    /// <param name="fieldPath">The path to the field to set. Supports dot notation for nested fields.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The response instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public static IResponse SetFieldIfNotExists(this IResponse response, string fieldPath, object? value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        if (response.Get<object>(fieldPath) == null)
        {
            response.UpdateField(fieldPath, value);
        }

        return response;
    }

    /// <summary>
    /// Merges additional data into the response data dictionary.
    /// Existing fields are overwritten if they exist in the additional data.
    /// </summary>
    /// <param name="response">The response instance.</param>
    /// <param name="additionalData">The data to merge into the response.</param>
    /// <returns>The response instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response or additionalData is null.</exception>
    public static IResponse MergeData(this IResponse response, IDictionary<string, object> additionalData)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(additionalData, nameof(additionalData));

        foreach (var kvp in additionalData)
        {
            response.UpdateField(kvp.Key, kvp.Value);
        }

        return response;
    }

    /// <summary>
    /// Checks if a specific field exists in the response data.
    /// </summary>
    /// <param name="response">The response instance.</param>
    /// <param name="fieldPath">The path to the field to check. Supports dot notation for nested fields.</param>
    /// <returns><c>true</c> if the field exists; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if response is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public static bool HasField(this IResponse response, string fieldPath)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return response.Get<object>(fieldPath) != null;
    }
}
