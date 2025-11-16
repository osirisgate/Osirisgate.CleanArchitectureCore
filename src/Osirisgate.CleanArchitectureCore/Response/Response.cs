using Osirisgate.CleanArchitectureCore.Enums;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Response;

/// <summary>
/// Represents the standard implementation of the <see cref="IResponse"/> interface.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class Response : IResponse
{
    private bool _success;
    private string _message;
    private StatusCode _statusCode;
    private IDictionary<string, object> _data;
    private IDictionary<string, object> _meta;

    private Response(bool success, StatusCode statusCode, string message, IDictionary<string, object>? data)
    {
        _success = success;
        _statusCode = statusCode;
        _message = message;
        _data = data ?? new Dictionary<string, object>();
        _meta = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Response"/> class.
    /// </summary>
    /// <param name="success">Indicates whether the operation was successful.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="message">The response message.</param>
    /// <param name="data">The data payload of the response.</param>
    /// <returns>A new <see cref="IResponse"/> instance.</returns>
    public static IResponse Create(bool success, StatusCode statusCode, string message, IDictionary<string, object>? data)
    {
        return new Response(success, statusCode, message, data);
    }

    /// <inheritdoc/>
    public bool IsSuccess()
    {
        return _success;
    }

    /// <inheritdoc/>
    public int GetStatusCode()
    {
        return _statusCode.GetValue();
    }

    /// <inheritdoc/>
    public string GetMessage()
    {
        return _message;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetData()
    {
        return _data;
    }

    /// <inheritdoc/>
    public T? Get<T>(string fieldPath, object? defaultValue = null)
    {
        return Helper.Helper.Get<T>(_data, fieldPath, defaultValue);
    }

    /// <inheritdoc/>
    public void UpdateField(string fieldPath, object? value)
    {
        Helper.Helper.UpdateField(_data, fieldPath, value);
    }

    /// <inheritdoc/>
    public void SetSuccess(bool success)
    {
        _success = success;
    }

    /// <inheritdoc/>
    public void SetMessage(string message)
    {
        _message = message;
    }

    /// <inheritdoc/>
    public void SetStatusCode(StatusCode statusCode)
    {
        _statusCode = statusCode;
    }

    /// <inheritdoc/>
    public void ReplaceData(IDictionary<string, object> data)
    {
        _data = data;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetOutput()
    {
        var output = new Dictionary<string, object>
        {
            ["status"] = GetStatusLabel(),
            ["code"] = _statusCode.GetValue(),
            ["message"] = _message,
        };

        foreach (var entry in MapDataKeyAccordingToResponseStatus())
        {
            output[entry.Key] = entry.Value;
        }

        output["meta"] = new Dictionary<string, object>(_meta);

        return output;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetMeta()
    {
        return _meta;
    }

    /// <inheritdoc/>
    public void SetMeta(IDictionary<string, object> meta)
    {
        ArgumentNullException.ThrowIfNull(meta, nameof(meta));
        _meta = new Dictionary<string, object>(meta);
    }

    /// <inheritdoc/>
    public void UpdateMetaField(string fieldPath, object? value)
    {
        Helper.Helper.UpdateField(_meta, fieldPath, value);
    }

    /// <inheritdoc/>
    public T? GetMeta<T>(string fieldPath, object? defaultValue = null)
    {
        return Helper.Helper.Get<T>(_meta, fieldPath, defaultValue);
    }

    private string GetStatusLabel()
    {
        return _success ? Status.Success.GetValue() : Status.Error.GetValue();
    }

    private Dictionary<string, object> MapDataKeyAccordingToResponseStatus()
    {
        return new Dictionary<string, object>
        {
            [IsSuccess() ? "data" : "details"] = GetData(),
        };
    }
}
