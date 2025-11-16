using System.Text.Json;
using System.Text.Json.Serialization;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidCastException = Osirisgate.CleanArchitectureCore.Exception.InvalidCastException;

namespace Osirisgate.CleanArchitectureCore.Helper;

/// <summary>
/// Provides utility methods for dictionary operations, type casting, and DTO mapping.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class Helper
{
    private static readonly JsonSerializerOptions _dtoMappingOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    /// <summary>
    /// Retrieves a value from a nested dictionary using a dot-separated field path.
    /// Returns the default value if the path does not exist or types do not match.
    /// </summary>
    /// <param name="data">The data object (typically a dictionary).</param>
    /// <param name="fieldPath">The dot-separated field path to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the field is not found.</param>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <returns>The field's value, or the default value if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if data or fieldPath is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is empty or whitespace.</exception>
    public static T? Get<T>(object data, string fieldPath, object? defaultValue = null)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        var keys = fieldPath.Split('.');

        foreach (var key in keys)
        {
            if (data is not Dictionary<string, object> dict || !dict.TryGetValue(key, out var value))
            {
                return (T?)defaultValue;
            }

            data = value;
        }

        return data is T typedData ? typedData : (T?)defaultValue;
    }

    /// <summary>
    /// Retrieves a value from a nested dictionary using a dot-separated field path with case-insensitive key matching.
    /// First tries exact match, then normalized key match (snake_case/camelCase to PascalCase), then case-insensitive match.
    /// Returns the default value if the path does not exist or types do not match.
    /// </summary>
    /// <param name="data">The data object (typically a dictionary).</param>
    /// <param name="fieldPath">The dot-separated field path to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the field is not found.</param>
    /// <typeparam name="T">The type to cast the field value to.</typeparam>
    /// <returns>The field's value, or the default value if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if data or fieldPath is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is empty or whitespace.</exception>
    public static T? GetCaseInsensitive<T>(object data, string fieldPath, object? defaultValue = null)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        var keys = fieldPath.Split('.');

        foreach (var key in keys)
        {
            if (data is not Dictionary<string, object> dict)
            {
                return (T?)defaultValue;
            }

            if (!dict.TryGetValue(key, out var value))
            {
                var normalizedKey = ConvertSnakeCaseToPascalCase(key);
                if (!dict.TryGetValue(normalizedKey, out value))
                {
                    var found = false;
                    foreach (var kvp in dict)
                    {
                        if (string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase))
                        {
                            value = kvp.Value;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return (T?)defaultValue;
                    }
                }
            }

            data = value!;
        }

        return data is T typedData ? typedData : (T?)defaultValue;
    }

    /// <summary>
    /// Adds or merges a value into a nested dictionary using a dot-separated path.
    /// If the key exists and both values are dictionaries, they are merged recursively.
    /// Otherwise, the value is overwritten.
    /// </summary>
    /// <param name="data">The dictionary to update.</param>
    /// <param name="fieldPath">The dot-separated field path to update.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">Thrown if data or fieldPath is null.</exception>
    /// <exception cref="ArgumentException">Thrown if fieldPath is empty or whitespace.</exception>
    public static void UpdateField(IDictionary<string, object> data, string fieldPath, object? value)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath, nameof(fieldPath));

        var keys = fieldPath.Split('.');
        var current = data;

        for (var i = 0; i < keys.Length; i++)
        {
            var key = keys[i];

            if (i == keys.Length - 1)
            {
                if (current.TryGetValue(key, out var existing) &&
                    existing is Dictionary<string, object> existingDict &&
                    value is Dictionary<string, object> newDict)
                {
                    MergeDictionaries(existingDict, newDict);
                }
                else
                {
                    current[key] = value!;
                }
            }
            else
            {
                if (!current.TryGetValue(key, out var nested) || nested is not Dictionary<string, object> nestedDict)
                {
                    nestedDict = [];
                    current[key] = nestedDict;
                }
                current = nestedDict;
            }
        }
    }

    /// <summary>
    /// Recursively merges the source dictionary into the target dictionary.
    /// Existing dictionary keys are merged recursively, other values are overwritten.
    /// </summary>
    /// <param name="target">The target dictionary to merge into.</param>
    /// <param name="source">The source dictionary to merge from.</param>
    /// <exception cref="ArgumentNullException">Thrown if target or source is null.</exception>
    public static void MergeDictionaries(Dictionary<string, object> target, Dictionary<string, object> source)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        foreach (var kvp in source)
        {
            if (target.TryGetValue(kvp.Key, out var existing) &&
                existing is Dictionary<string, object> existingDict &&
                kvp.Value is Dictionary<string, object> newDict)
            {
                MergeDictionaries(existingDict, newDict);
            }
            else
            {
                target[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// Safely casts an object to a specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="value">The value to cast.</param>
    /// <returns>The casted value.</returns>
    /// <exception cref="InvalidCastException">Thrown when the cast is not possible.</exception>
    public static T CastTo<T>(object value)
    {
        if (value is T typedValue)
            return typedValue;

        var sourceTypeName = value?.GetType().FullName ?? "null";
        var targetTypeName = typeof(T).FullName ?? typeof(T).Name;

        throw InvalidCastException.Create(sourceTypeName, targetTypeName, value);
    }

    /// <summary>
    /// Converts snake_case or lowercase strings to PascalCase.
    /// Examples: "email" -> "Email", "first_name" -> "FirstName", "userName" -> "UserName"
    /// </summary>
    /// <param name="name">The string to convert.</param>
    /// <returns>The converted string in PascalCase.</returns>
    /// <exception cref="ArgumentNullException">Thrown if name is null.</exception>
    public static string ConvertSnakeCaseToPascalCase(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        if (string.IsNullOrEmpty(name))
            return name;

        if (char.IsUpper(name[0]))
            return name;

        if (name.Contains('_'))
        {
            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var result = new System.Text.StringBuilder();
            foreach (var part in parts)
            {
                if (part.Length > 0)
                {
                    result.Append(char.ToUpperInvariant(part[0]));
                    if (part.Length > 1)
                    {
                        result.Append(part[1..].ToLowerInvariant());
                    }
                }
            }
            return result.ToString();
        }

        return char.ToUpperInvariant(name[0]) + name[1..];
    }

    /// <summary>
    /// Normalizes dictionary keys to PascalCase recursively.
    /// Keys are converted from snake_case or lowercase to PascalCase (e.g., "email" -> "Email", "first_name" -> "FirstName").
    /// </summary>
    /// <param name="payload">The dictionary to normalize.</param>
    /// <returns>A new dictionary with normalized keys.</returns>
    /// <exception cref="ArgumentNullException">Thrown if payload is null.</exception>
    public static Dictionary<string, object> NormalizePayloadKeys(IDictionary<string, object> payload)
    {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        var normalized = new Dictionary<string, object>(payload.Count);

        foreach (var (key, value) in payload)
        {
            var normalizedKey = ConvertSnakeCaseToPascalCase(key);

            if (value is IDictionary<string, object> nestedDict)
            {
                normalized[normalizedKey] = NormalizePayloadKeys(nestedDict);
            }
            else if (value is IReadOnlyDictionary<string, object> nestedReadOnlyDict)
            {
                normalized[normalizedKey] = NormalizePayloadKeys(new Dictionary<string, object>(nestedReadOnlyDict));
            }
            else
            {
                normalized[normalizedKey] = value;
            }
        }

        return normalized;
    }

    /// <summary>
    /// Filters a payload dictionary based on a structure definition.
    /// Only fields defined in the structure are included in the result.
    /// </summary>
    /// <param name="sourcePayload">The source payload to filter.</param>
    /// <param name="structure">The structure definition (typically RequestStructure).</param>
    /// <param name="targetPayload">The target dictionary to populate with filtered fields.</param>
    /// <exception cref="ArgumentNullException">Thrown if sourcePayload, structure, or targetPayload is null.</exception>
    public static void FilterPayloadByStructure(
        IReadOnlyDictionary<string, object> sourcePayload,
        IDictionary<string, object> structure,
        IDictionary<string, object> targetPayload)
    {
        ArgumentNullException.ThrowIfNull(sourcePayload, nameof(sourcePayload));
        ArgumentNullException.ThrowIfNull(structure, nameof(structure));
        ArgumentNullException.ThrowIfNull(targetPayload, nameof(targetPayload));

        foreach (var (fieldName, rule) in structure)
        {
            if (rule is IDictionary<string, object> nestedStructure)
            {
                if (sourcePayload.TryGetValue(fieldName, out var nestedValue) &&
                    nestedValue is IReadOnlyDictionary<string, object> nestedDict)
                {
                    var nestedTarget = new Dictionary<string, object>();
                    FilterPayloadByStructure(nestedDict, nestedStructure, nestedTarget);
                    if (nestedTarget.Count > 0)
                    {
                        targetPayload[fieldName] = nestedTarget;
                    }
                }
            }
            else
            {
                if (sourcePayload.TryGetValue(fieldName, out var value))
                {
                    targetPayload[fieldName] = value;
                }
            }
        }
    }

    /// <summary>
    /// Maps a dictionary payload to a DTO type using System.Text.Json for optimal performance.
    /// The mapping supports case-insensitive property matching and snake_case to PascalCase conversion.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to map to.</typeparam>
    /// <param name="payload">The payload dictionary (should already be normalized).</param>
    /// <returns>An instance of the DTO type with values from the payload, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if payload is null.</exception>
    public static TDto? MapToDto<TDto>(IDictionary<string, object> payload) where TDto : new()
    {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(payload, _dtoMappingOptions);
        return JsonSerializer.Deserialize<TDto>(jsonBytes, _dtoMappingOptions);
    }
}
