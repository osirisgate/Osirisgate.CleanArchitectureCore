namespace Osirisgate.CleanArchitectureCore.Response;

/// <summary>
/// Defines the contract for a response object, which encapsulates the result of a use case.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IResponse
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
    public bool IsSuccess();

    /// <summary>
    /// Gets the HTTP status code for the response.
    /// </summary>
    /// <returns>An integer representing the HTTP status code.</returns>
    public int GetStatusCode();

    /// <summary>
    /// Gets the response message.
    /// </summary>
    /// <returns>The response message string.</returns>
    public string GetMessage();

    /// <summary>
    /// Gets a specific field from the response data, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldPath">The name of the field to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the field is not found.</param>
    /// <returns>The field's value, or null if not found.</returns>
    public T? Get<T>(string fieldPath, object? defaultValue = null);

    /// <summary>
    /// Gets the data payload of the response.
    /// </summary>
    /// <returns>A dictionary containing the response data.</returns>
    public IDictionary<string, object> GetData();

    /// <summary>
    /// Gets the entire response formatted as a standardized dictionary.
    /// </summary>
    /// <returns>A dictionary representing the final, formatted response.</returns>
    public IDictionary<string, object> GetOutput();

    /// <summary>
    /// Sets the success status of the response.
    /// </summary>
    /// <param name="success">The success status to set.</param>
    public void SetSuccess(bool success);

    /// <summary>
    /// Sets the response message.
    /// </summary>
    /// <param name="message">The response message to set.</param>
    public void SetMessage(string message);

    /// <summary>
    /// Sets the HTTP status code for the response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to set.</param>
    public void SetStatusCode(StatusCode statusCode);

    /// <summary>
    /// Replace the data payload of the response.
    /// </summary>
    /// <param name="data">The data payload to set.</param>
    public void ReplaceData(IDictionary<string, object> data);

    /// <summary>
    /// Updates a specific field in the response data.
    /// </summary>
    /// <param name="fieldPath">The path to the field to update.</param>
    /// <param name="value">The new value for the field.</param>
    public void UpdateField(string fieldPath, object? value);

    /// <summary>
    /// Gets the meta dictionary containing metadata about the response.
    /// </summary>
    /// <returns>A dictionary containing metadata.</returns>
    public IDictionary<string, object> GetMeta();

    /// <summary>
    /// Sets the entire meta dictionary.
    /// </summary>
    /// <param name="meta">The meta dictionary to set.</param>
    public void SetMeta(IDictionary<string, object> meta);

    /// <summary>
    /// Updates a specific field in the meta dictionary.
    /// </summary>
    /// <param name="fieldPath">The path to the field to update. Supports dot notation for nested fields.</param>
    /// <param name="value">The new value for the field.</param>
    public void UpdateMetaField(string fieldPath, object? value);

    /// <summary>
    /// Gets a specific field from the meta dictionary.
    /// </summary>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <param name="fieldPath">The name of the field to retrieve. Supports dot notation for nested fields.</param>
    /// <param name="defaultValue">The default value to return if the field is not found.</param>
    /// <returns>The field's value, or null if not found.</returns>
    public T? GetMeta<T>(string fieldPath, object? defaultValue = null);
}
