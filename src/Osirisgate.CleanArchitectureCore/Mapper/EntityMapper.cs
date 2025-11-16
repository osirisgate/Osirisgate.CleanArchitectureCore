using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidCastException = Osirisgate.CleanArchitectureCore.Exception.InvalidCastException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Mapper;

/// <summary>
/// Provides methods to convert between domain entities and persistence entities (DTOs).
/// Supports mapping only when properties are public with public setters.
/// For complex mappings (private setters, factory methods, etc.), use custom mapping functions.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class EntityMapper
{
    private static readonly JsonSerializerOptions _mappingOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    // Cache for property mappings to improve performance
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _sourcePropertiesCache = new();
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _targetPropertiesCache = new();

    /// <summary>
    /// Maps a domain entity to a persistence entity (DTO) using JSON serialization.
    /// Only works with public properties that have public setters.
    /// </summary>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <param name="domainEntity">The domain entity to map.</param>
    /// <returns>A new instance of the persistence entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if domainEntity is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if mapping fails due to incompatible types or missing setters.</exception>
    public static TPersistence ToPersistence<TDomain, TPersistence>(TDomain domainEntity)
        where TPersistence : new()
    {
        ArgumentNullException.ThrowIfNull(domainEntity, nameof(domainEntity));

        try
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(domainEntity, _mappingOptions);
            var deserialized = JsonSerializer.Deserialize<TPersistence>(jsonBytes, _mappingOptions);

            if (deserialized != null)
            {
                return deserialized;
            }

            throw InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = "entitymapper.mapping.failed",
                ["details"] = new Dictionary<string, object>
                {
                    ["message"] = "entitymapper.mapping.failed",
                    ["reason"] = "Deserialization returned null"
                }
            });
        }
        catch (JsonException ex)
        {
            throw InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = "entitymapper.mapping.failed",
                ["details"] = new Dictionary<string, object>
                {
                    ["message"] = "entitymapper.mapping.failed",
                    ["exception"] = ex
                }
            });
        }
    }

    /// <summary>
    /// Maps a persistence entity (DTO) to a domain entity using JSON serialization.
    /// Only works with public properties that have public setters.
    /// </summary>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <param name="persistenceEntity">The persistence entity to map.</param>
    /// <returns>A new instance of the domain entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if persistenceEntity is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if mapping fails due to incompatible types or missing setters.</exception>
    public static TDomain ToDomain<TPersistence, TDomain>(TPersistence persistenceEntity)
        where TDomain : new()
    {
        ArgumentNullException.ThrowIfNull(persistenceEntity, nameof(persistenceEntity));

        try
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(persistenceEntity, _mappingOptions);
            var deserialized = JsonSerializer.Deserialize<TDomain>(jsonBytes, _mappingOptions);

            if (deserialized != null)
            {
                return deserialized;
            }

            throw InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = "entitymapper.mapping.failed",
                ["details"] = new Dictionary<string, object>
                {
                    ["message"] = "entitymapper.mapping.failed",
                    ["reason"] = "Deserialization returned null"
                }
            });
        }
        catch (JsonException ex)
        {
            throw InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = "entitymapper.mapping.failed",
                ["details"] = new Dictionary<string, object>
                {
                    ["message"] = "entitymapper.mapping.failed",
                    ["exception"] = ex
                }
            });
        }
    }

    /// <summary>
    /// Maps a collection of domain entities to a collection of persistence entities (DTOs).
    /// </summary>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <param name="domainEntities">The collection of domain entities to map.</param>
    /// <returns>A new list of persistence entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if domainEntities is null.</exception>
    public static List<TPersistence> ToPersistenceList<TDomain, TPersistence>(IEnumerable<TDomain> domainEntities)
        where TPersistence : new()
    {
        ArgumentNullException.ThrowIfNull(domainEntities, nameof(domainEntities));

        return [.. domainEntities.Select(ToPersistence<TDomain, TPersistence>)];
    }

    /// <summary>
    /// Maps a collection of persistence entities (DTOs) to a collection of domain entities.
    /// </summary>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <param name="persistenceEntities">The collection of persistence entities to map.</param>
    /// <returns>A new list of domain entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if persistenceEntities is null.</exception>
    public static List<TDomain> ToDomainList<TPersistence, TDomain>(IEnumerable<TPersistence> persistenceEntities)
        where TDomain : new()
    {
        ArgumentNullException.ThrowIfNull(persistenceEntities, nameof(persistenceEntities));

        return [.. persistenceEntities.Select(ToDomain<TPersistence, TDomain>)];
    }

    /// <summary>
    /// Maps a domain entity to a persistence entity using a custom mapping function.
    /// Use this when the automatic JSON mapping doesn't work (e.g., private setters, factory methods).
    /// </summary>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <param name="domainEntity">The domain entity to map.</param>
    /// <param name="mapper">The custom mapping function.</param>
    /// <returns>A new instance of the persistence entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if domainEntity or mapper is null.</exception>
    public static TPersistence MapToPersistence<TDomain, TPersistence>(TDomain domainEntity, Func<TDomain, TPersistence> mapper)
    {
        ArgumentNullException.ThrowIfNull(domainEntity, nameof(domainEntity));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        return mapper(domainEntity);
    }

    /// <summary>
    /// Maps a persistence entity to a domain entity using a custom mapping function.
    /// Use this when the automatic JSON mapping doesn't work (e.g., private setters, factory methods, constructors).
    /// </summary>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <param name="persistenceEntity">The persistence entity to map.</param>
    /// <param name="mapper">The custom mapping function.</param>
    /// <returns>A new instance of the domain entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if persistenceEntity or mapper is null.</exception>
    public static TDomain MapToDomain<TPersistence, TDomain>(TPersistence persistenceEntity, Func<TPersistence, TDomain> mapper)
    {
        ArgumentNullException.ThrowIfNull(persistenceEntity, nameof(persistenceEntity));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        return mapper(persistenceEntity);
    }

    /// <summary>
    /// Maps a collection of domain entities to a collection of persistence entities using a custom mapping function.
    /// </summary>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <param name="domainEntities">The collection of domain entities to map.</param>
    /// <param name="mapper">The custom mapping function.</param>
    /// <returns>A new list of persistence entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if domainEntities or mapper is null.</exception>
    public static List<TPersistence> MapListToPersistence<TDomain, TPersistence>(
        IEnumerable<TDomain> domainEntities,
        Func<TDomain, TPersistence> mapper)
    {
        ArgumentNullException.ThrowIfNull(domainEntities, nameof(domainEntities));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        return [.. domainEntities.Select(mapper)];
    }

    /// <summary>
    /// Maps a collection of persistence entities to a collection of domain entities using a custom mapping function.
    /// </summary>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <param name="persistenceEntities">The collection of persistence entities to map.</param>
    /// <param name="mapper">The custom mapping function.</param>
    /// <returns>A new list of domain entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if persistenceEntities or mapper is null.</exception>
    public static List<TDomain> MapListToDomain<TPersistence, TDomain>(
        IEnumerable<TPersistence> persistenceEntities,
        Func<TPersistence, TDomain> mapper)
    {
        ArgumentNullException.ThrowIfNull(persistenceEntities, nameof(persistenceEntities));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

        return [.. persistenceEntities.Select(mapper)];
    }

    /// <summary>
    /// Updates an existing persistence entity with values from a domain entity.
    /// Updates the existing object in place.
    /// Copies properties directly from source to allow null values to be set.
    /// </summary>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <param name="domainEntity">The domain entity containing the new values.</param>
    /// <param name="existingPersistenceEntity">The existing persistence entity to update.</param>
    /// <exception cref="ArgumentNullException">Thrown if domainEntity or existingPersistenceEntity is null.</exception>
    /// <exception cref="InvalidCastException">Thrown if a property conversion fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a property mapping fails.</exception>
    public static void UpdatePersistence<TDomain, TPersistence>(
        TDomain domainEntity,
        TPersistence existingPersistenceEntity)
        where TPersistence : new()
    {
        ArgumentNullException.ThrowIfNull(domainEntity, nameof(domainEntity));
        ArgumentNullException.ThrowIfNull(existingPersistenceEntity, nameof(existingPersistenceEntity));

        CopyProperties(domainEntity!, existingPersistenceEntity!);
    }

    /// <summary>
    /// Updates an existing domain entity with values from a persistence entity.
    /// Updates the existing object in place.
    /// Copies properties directly from source to allow null values to be set.
    /// </summary>
    /// <typeparam name="TPersistence">The persistence entity type.</typeparam>
    /// <typeparam name="TDomain">The domain entity type.</typeparam>
    /// <param name="persistenceEntity">The persistence entity containing the new values.</param>
    /// <param name="existingDomainEntity">The existing domain entity to update.</param>
    /// <exception cref="ArgumentNullException">Thrown if persistenceEntity or existingDomainEntity is null.</exception>
    /// <exception cref="InvalidCastException">Thrown if a property conversion fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a property mapping fails.</exception>
    public static void UpdateDomain<TPersistence, TDomain>(
        TPersistence persistenceEntity,
        TDomain existingDomainEntity)
        where TDomain : new()
    {
        ArgumentNullException.ThrowIfNull(persistenceEntity, nameof(persistenceEntity));
        ArgumentNullException.ThrowIfNull(existingDomainEntity, nameof(existingDomainEntity));

        CopyProperties(persistenceEntity!, existingDomainEntity!);
    }

    /// <summary>
    /// Copies properties from source to target using reflection with caching for performance.
    /// </summary>
    private static void CopyProperties(object source, object target)
    {
        if (source == null || target == null)
        {
            return;
        }

        var sourceType = source.GetType();
        var targetType = target.GetType();

        var sourceProperties = _sourcePropertiesCache.GetOrAdd(sourceType, type =>
            [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.GetIndexParameters().Length == 0)]);

        var targetProperties = _targetPropertiesCache.GetOrAdd(targetType, type =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase));

        // Note: We copy null values to allow clearing properties (setting them to null)
        foreach (var sourceProperty in sourceProperties)
        {
            if (targetProperties.TryGetValue(sourceProperty.Name, out var targetProperty))
            {
                var value = sourceProperty.GetValue(source);
                var sourcePropertyType = sourceProperty.PropertyType;
                var targetPropertyType = targetProperty.PropertyType;

                var isNullable = !targetPropertyType.IsValueType || Nullable.GetUnderlyingType(targetPropertyType) != null;

                if (value == null)
                {
                    if (isNullable)
                    {
                        targetProperty.SetValue(target, null);
                    }
                }
                else if (sourcePropertyType == targetPropertyType)
                {
                    targetProperty.SetValue(target, value);
                }
                else if (IsSimpleType(sourcePropertyType) && IsSimpleType(targetPropertyType))
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(value, targetPropertyType);
                        targetProperty.SetValue(target, convertedValue);
                    }
                    catch (System.Exception ex)
                    {
                        throw InvalidCastException.Create(new Dictionary<string, object>
                        {
                            ["message"] = "entitymapper.conversion.failed",
                            ["details"] = new Dictionary<string, object>
                            {
                                ["message"] = "entitymapper.conversion.failed",
                                ["property_name"] = sourceProperty.Name,
                                ["source_type"] = sourcePropertyType.FullName ?? sourcePropertyType.Name,
                                ["target_type"] = targetPropertyType.FullName ?? targetPropertyType.Name,
                                ["value"] = value?.ToString() ?? "null",
                                ["exception"] = ex
                            }
                        });
                    }
                }
                else
                {
                    try
                    {
                        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(value, _mappingOptions);
                        var deserialized = JsonSerializer.Deserialize(jsonBytes, targetPropertyType, _mappingOptions);
                        if (deserialized != null)
                        {
                            targetProperty.SetValue(target, deserialized);
                        }
                        else
                        {
                            throw InvalidOperationException.Create(new Dictionary<string, object>
                            {
                                ["message"] = "entitymapper.mapping.failed",
                                ["details"] = new Dictionary<string, object>
                                {
                                    ["message"] = "entitymapper.mapping.failed",
                                    ["property_name"] = sourceProperty.Name,
                                    ["source_type"] = sourcePropertyType.FullName ?? sourcePropertyType.Name,
                                    ["target_type"] = targetPropertyType.FullName ?? targetPropertyType.Name,
                                    ["reason"] = "Deserialization returned null"
                                }
                            });
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Re-throw InvalidOperationException as-is
                        throw;
                    }
                    catch (System.Exception ex)
                    {
                        // Mapping failed, throw exception
                        throw InvalidOperationException.Create(new Dictionary<string, object>
                        {
                            ["message"] = "entitymapper.mapping.failed",
                            ["details"] = new Dictionary<string, object>
                            {
                                ["message"] = "entitymapper.mapping.failed",
                                ["property_name"] = sourceProperty.Name,
                                ["source_type"] = sourcePropertyType.FullName ?? sourcePropertyType.Name,
                                ["target_type"] = targetPropertyType.FullName ?? targetPropertyType.Name,
                                ["exception"] = ex
                            }
                        });
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if a type is a simple type (primitive, string, decimal, DateTime, etc.).
    /// </summary>
    private static bool IsSimpleType(Type type)
    {
        if (type.IsPrimitive)
        {
            return true;
        }

        if (type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(Guid))
        {
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return IsSimpleType(underlyingType);
        }

        return false;
    }
}
