namespace Osirisgate.CleanArchitectureCore.Request;

/// <summary>
/// Defines the contract for a request object, which validates and provides access to input data.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IRequest
{
    /// <summary>
    /// Gets the entire validated payload.
    /// </summary>
    /// <returns>A dictionary representing the validated payload.</returns>
    public IReadOnlyDictionary<string, object> GetPayload();

    /// <summary>
    /// Gets the entire modified payload.
    /// </summary>
    /// <returns>A dictionary representing the modified payload.</returns>
    public IDictionary<string, object> GetModifiedPayload();

    /// <summary>
    /// Sets the modified payload.
    /// </summary>
    /// <param name="modifiedPayload">The modified payload to set.</param>
    /// <returns>The modified request instance.</returns>
    public IRequest ModifiedPayload(IDictionary<string, object> modifiedPayload);

    /// <summary>
    /// Gets a specific field from the payload, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldName">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <param name="defaultValue">An optional default value to return if the field is not found.</param>
    /// <returns>The field's value, or the default value if not found.</returns>
    public T? GetField<T>(string fieldName, object? defaultValue = null);

    /// <summary>
    /// Gets a specific field from the modified payload, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <param name="defaultValue">An optional default value to return if the field is not found.</param>
    /// <returns>The field's value, or the default value if not found.</returns>
    public T? GetModifiedField<T>(string fieldPath, object? defaultValue = null);

    /// <summary>
    /// Validates the request by applying additional constraints asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    /// <exception cref="BadRequestContentException">Thrown if validation fails.</exception>
    public Task ValidateAsync(CancellationToken cancellationToken = default);
}
