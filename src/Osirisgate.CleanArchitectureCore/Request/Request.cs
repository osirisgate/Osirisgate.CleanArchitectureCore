using Osirisgate.CleanArchitectureCore.Exception;
using static Osirisgate.CleanArchitectureCore.Helper.Helper;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Request;

/// <summary>
/// Abstract base class for request objects that validate and manage input data.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public abstract class Request : IRequest
{
    /// <summary>
    /// The message to be used when a required field type does not match the expected type.
    /// </summary>
    private const string RequiredFieldTypeMismatchMessage = "required field type not matching [Dictionary<string, object>]";

    /// <summary>
    /// The structure of the expected request payload.
    /// Override in derived classes to define validation rules. Required fields should be marked as 'true'.
    /// </summary>
    protected virtual IDictionary<string, object> RequestStructure => new Dictionary<string, object>();

    private readonly IReadOnlyDictionary<string, object> _payload;
    private readonly IDictionary<string, object> _modifiedPayload = new Dictionary<string, object>();
    private readonly Lazy<IDictionary<string, object>> _normalizedPayload;
    private Lazy<IDictionary<string, object>>? _normalizedModifiedPayload;

    /// <summary>
    /// Initializes a new request with the given payload, performing all validation.
    /// Throws exceptions if required or unauthorized fields are present.
    /// </summary>
    /// <param name="payload">The request payload.</param>
    /// <exception cref="BadRequestContentException">Thrown if payload contains missing or unauthorized fields.</exception>
    protected Request(IReadOnlyDictionary<string, object> payload)
    {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        var requestStructure = RequestStructure;

        if (requestStructure.Count == 0)
        {
            _payload = payload;
        }
        else
        {
            var (missingFields, unauthorizedFields) = RequestPayloadFilter(payload, requestStructure);

            if (missingFields.Count > 0)
            {
                ThrowMissingFieldsIfAny(missingFields);
            }

            if (unauthorizedFields.Count > 0)
            {
                ThrowUnRequiredFieldsIfAny(unauthorizedFields);
            }

            _payload = payload;
        }

        _normalizedPayload = new Lazy<IDictionary<string, object>>(
            ToDtoInternal,
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object> GetPayload() => _payload;

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public T? GetField<T>(string fieldPath, object? defaultValue = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        return GetCaseInsensitive<T>(_normalizedPayload.Value, fieldPath, defaultValue);
    }

    /// <inheritdoc/>
    public IDictionary<string, object> GetModifiedPayload() => _modifiedPayload;

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if fieldPath is null or empty.</exception>
    public T? GetModifiedField<T>(string fieldPath, object? defaultValue = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        var normalizedModified = LazyInitializer.EnsureInitialized(
            ref _normalizedModifiedPayload,
            () => new Lazy<IDictionary<string, object>>(
                ToModifiedDtoInternal,
                LazyThreadSafetyMode.ExecutionAndPublication)
        );

        return GetCaseInsensitive<T>(normalizedModified.Value, fieldPath, defaultValue);
    }

    /// <inheritdoc/>
    public IRequest ModifiedPayload(IDictionary<string, object> modifiedPayload)
    {
        ArgumentNullException.ThrowIfNull(modifiedPayload, nameof(modifiedPayload));

        if (modifiedPayload.Count == 0)
            return this;

        MergeDictionaries(
            CastTo<Dictionary<string, object>>(_modifiedPayload),
            CastTo<Dictionary<string, object>>(modifiedPayload)
        );

        _normalizedModifiedPayload = null;

        return this;
    }

    /// <inheritdoc/>
    public virtual async Task ValidateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await ApplyConstraintsOnRequestFieldsAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a normalized dictionary representation of the request payload based on RequestStructure.
    /// Keys are normalized to PascalCase (e.g., "email" -> "Email", "first_name" -> "FirstName").
    /// This dictionary can be used directly with libraries like FluentValidation or can be converted to any DTO type.
    /// </summary>
    /// <returns>A dictionary with normalized keys matching RequestStructure fields.</returns>
    protected IDictionary<string, object> ToDto()
    {
        return _normalizedPayload.Value;
    }

    private Dictionary<string, object> ToDtoInternal()
    {
        var requestStructure = RequestStructure;
        var payload = GetPayload();

        IDictionary<string, object> filteredPayload;
        if (requestStructure.Count > 0)
        {
            filteredPayload = new Dictionary<string, object>();
            FilterPayloadByStructure(payload, requestStructure, filteredPayload);
        }
        else
        {
            filteredPayload = new Dictionary<string, object>(payload);
        }

        return NormalizePayloadKeys(filteredPayload);
    }

    /// <summary>
    /// Maps the request payload to a DTO type using System.Text.Json for optimal performance.
    /// The mapping is based solely on RequestStructure fields, ignoring any JsonPropertyName attributes
    /// in the DTO. Property names are matched case-insensitively and support snake_case to PascalCase conversion.
    /// This allows using existing DTO classes without needing to recreate them or modify their attributes.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to map to. Can be any existing DTO class.</typeparam>
    /// <returns>An instance of the DTO type with values from the payload, mapped based on RequestStructure.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the mapping to DTO fails.</exception>
    protected TDto ToDto<TDto>() where TDto : new()
    {
        var normalizedPayload = ToDto();
        return MapToDto<TDto>(normalizedPayload) ?? throw InvalidOperationException.Create(
            $"Failed to map payload to DTO type {typeof(TDto).Name}. This may occur if the DTO structure does not match the RequestStructure fields.");
    }

    /// <summary>
    /// Gets a normalized dictionary representation of the modified payload.
    /// Keys are normalized to PascalCase (e.g., "email" -> "Email", "first_name" -> "FirstName").
    /// This dictionary can be used directly with libraries like FluentValidation or can be converted to any DTO type.
    /// </summary>
    /// <returns>A dictionary with normalized keys from the modified payload.</returns>
    protected IDictionary<string, object> ToModifiedDto()
    {
        var normalizedModified = LazyInitializer.EnsureInitialized(
            ref _normalizedModifiedPayload,
            () => new Lazy<IDictionary<string, object>>(
                ToModifiedDtoInternal,
                LazyThreadSafetyMode.ExecutionAndPublication)
        );

        return normalizedModified.Value;
    }

    private Dictionary<string, object> ToModifiedDtoInternal()
    {
        var modifiedPayload = GetModifiedPayload();
        return NormalizePayloadKeys(modifiedPayload);
    }

    /// <summary>
    /// Can be overridden to add custom validation logic on request fields.
    /// Called after structure validation is complete.
    /// This method is called asynchronously via <see cref="ValidateAsync"/> or factory methods.
    /// The payload is accessible via <see cref="GetPayload"/>, <see cref="GetField{T}"/>, <see cref="ToDto()"/>, or <see cref="ToDto{TDto}()"/>.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    protected virtual Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    private static (Dictionary<string, string> MissingFields, List<string> UnauthorizedFields) RequestPayloadFilter(
        IReadOnlyDictionary<string, object> payload,
        IDictionary<string, object> requestStructure)
    {
        var missingFields = FindMissingFields(requestStructure, payload, string.Empty);
        var unauthorizedFields = FindUnAuthorizedFields(payload, requestStructure, string.Empty);
        return (missingFields, unauthorizedFields);
    }

    private static Dictionary<string, string> FindMissingFields(
        IDictionary<string, object> authorizedFields,
        IReadOnlyDictionary<string, object> payload,
        string prefix)
    {
        var missing = new Dictionary<string, string>(authorizedFields.Count);

        foreach (var (field, rule) in authorizedFields)
        {
            var fullKey = string.IsNullOrEmpty(prefix) ? field : $"{prefix}{field}";
            var hasField = payload.TryGetValue(field, out var value);

            if (rule is bool isRequired)
            {
                if (isRequired && !hasField)
                {
                    missing[fullKey] = "required";
                }
                continue;
            }

            if (rule is IDictionary<string, object> nestedRules)
            {
                if (!hasField)
                {
                    missing[fullKey] = RequiredFieldTypeMismatchMessage;
                    continue;
                }

                if (value is IReadOnlyDictionary<string, object> nestedPayloadDict)
                {
                    var nestedPrefix = $"{fullKey}.";
                    var nestedMissing = FindMissingFields(nestedRules, nestedPayloadDict, nestedPrefix);
                    foreach (var m in nestedMissing)
                    {
                        missing[m.Key] = m.Value;
                    }
                }
                else
                {
                    missing[fullKey] = RequiredFieldTypeMismatchMessage;
                }
            }
        }

        return missing;
    }

    private static List<string> FindUnAuthorizedFields(
        IReadOnlyDictionary<string, object> payload,
        IDictionary<string, object> authorizedFields,
        string prefix)
    {
        var unAuthorized = new List<string>(Math.Min(payload.Count, 16));

        foreach (var (field, value) in payload)
        {
            if (!authorizedFields.TryGetValue(field, out var rule))
            {
                unAuthorized.Add(string.IsNullOrEmpty(prefix) ? field : $"{prefix}{field}");
                continue;
            }

            if (value is IReadOnlyDictionary<string, object> nestedPayload &&
                rule is IDictionary<string, object> nestedRule)
            {
                var nestedPrefix = string.IsNullOrEmpty(prefix) ? $"{field}." : $"{prefix}{field}.";
                var nestedUnauthorized = FindUnAuthorizedFields(nestedPayload, nestedRule, nestedPrefix);
                if (nestedUnauthorized.Count > 0)
                {
                    unAuthorized.AddRange(nestedUnauthorized);
                }
            }
        }

        return unAuthorized;
    }

    private static void ThrowMissingFieldsIfAny(Dictionary<string, string> missingFields)
    {
        if (missingFields.Count == 0)
            return;

        throw new BadRequestContentException(new Dictionary<string, object>
        {
            ["message"] = "missing.required.fields",
            ["details"] = new Dictionary<string, object> { ["missing_fields"] = missingFields }
        });
    }

    private static void ThrowUnRequiredFieldsIfAny(List<string> unauthorizedFields)
    {
        if (unauthorizedFields.Count == 0)
            return;

        throw new BadRequestContentException(new Dictionary<string, object>
        {
            ["message"] = "illegal.fields",
            ["details"] = new Dictionary<string, object> { ["unrequired_fields"] = unauthorizedFields }
        });
    }
}
