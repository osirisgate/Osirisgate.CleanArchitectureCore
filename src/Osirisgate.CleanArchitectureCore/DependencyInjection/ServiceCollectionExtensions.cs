using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Pipeline;
using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;
using PresenterBase = Osirisgate.CleanArchitectureCore.Presenter.Presenter;
using ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime;
using UsecaseBase = Osirisgate.CleanArchitectureCore.Usecase.Usecase;

namespace Osirisgate.CleanArchitectureCore.DependencyInjection;

/// <summary>
/// Collects registration errors to be thrown at the end of the registration process.
/// </summary>
internal sealed class RegistrationErrorCollector
{
    private readonly List<InvalidOperationException> _errors = [];

    public void AddError(InvalidOperationException error)
    {
        _errors.Add(error);
    }

    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Extracts the component type from error details or falls back to parsing the message.
    /// </summary>
    private static string ExtractComponentType(IDictionary<string, object>? errorDetails, string? message)
    {
        if (errorDetails != null && errorDetails.TryGetValue("componentKind", out var componentKindObj))
        {
            var componentKind = componentKindObj?.ToString();
            if (!string.IsNullOrWhiteSpace(componentKind))
            {
                return componentKind.ToLowerInvariant();
            }
        }

        // Fallback: parse message (for backward compatibility)
        if (string.IsNullOrWhiteSpace(message))
        {
            return "unknown";
        }

        var firstDot = message.IndexOf('.');
        if (firstDot > 0)
        {
            return message[..firstDot];
        }

        return message;
    }

    public void ThrowIfHasErrors()
    {
        if (_errors.Count == 0)
        {
            return;
        }

        if (_errors.Count == 1)
        {
            throw _errors[0];
        }

        // Group errors by component type for better readability
        var errorsByComponent = new Dictionary<string, List<IDictionary<string, object>>>();
        var allMessages = new List<string>();

        foreach (var error in _errors)
        {
            var formatted = error.Format();
            var message = formatted.TryGetValue("message", out var msg) ? msg?.ToString() ?? error.Message : error.Message;

            var errorDetailsDict = formatted.TryGetValue("details", out var details) && details is IDictionary<string, object> detailsDict
                ? detailsDict
                : null;

            var componentType = ExtractComponentType(errorDetailsDict, message);

            allMessages.Add(message);

            var errorDetails = new Dictionary<string, object>
            {
                ["message"] = message
            };

            if (errorDetailsDict != null)
            {
                // Copy all details from the original error
                foreach (var kvp in errorDetailsDict)
                {
                    errorDetails[kvp.Key] = kvp.Value;
                }
            }

            if (!errorsByComponent.TryGetValue(componentType, out var componentErrors))
            {
                componentErrors = [];
                errorsByComponent[componentType] = componentErrors;
            }

            componentErrors.Add(errorDetails);
        }

        // Build a simplified and readable structure
        var groupedErrors = errorsByComponent.ToDictionary(
            kvp => kvp.Key,
            kvp => new Dictionary<string, object>
            {
                ["count"] = kvp.Value.Count,
                ["errors"] = kvp.Value
            }
        );

        throw InvalidOperationException.Create(new Dictionary<string, object>
        {
            ["message"] = "multiple.registration.errors",
            ["details"] = new Dictionary<string, object>
            {
                ["errorCount"] = _errors.Count,
                ["componentTypes"] = errorsByComponent.Keys.ToList(),
                ["errorsByComponent"] = groupedErrors,
                ["allMessages"] = allMessages,
                ["reason"] = $"Multiple registration errors occurred during component registration. {_errors.Count} error(s) found across {errorsByComponent.Count} component type(s).",
                ["suggestion"] = "Review the errors grouped by component type below. Ensure all components implement their declared interfaces and that there are no conflicting implementations in the same environment."
            }
        });
    }
}

/// <summary>
/// Extension methods for registering Clean Architecture components in the dependency injection container.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class ServiceCollectionExtensions
{
    private static readonly ConcurrentDictionary<string, Type[]> _assemblyTypesCache = new();
    private static readonly ConcurrentDictionary<(string TypeFullName, string? Environment), bool> _typeEnvironmentCache = new();
    private static readonly ConcurrentDictionary<string, Dictionary<string, Type>> _interfaceNameCache = new();

    /// <summary>
    /// Clears all internal caches used for component registration.
    /// Use this method to free memory when working with dynamically loaded/unloaded assemblies or in test scenarios.
    /// </summary>
    /// <remarks>
    /// This is useful in scenarios where:
    /// - Running unit tests that call AddCleanArchitectureCore many times
    /// - Working with a modular architecture that loads/unloads DLLs dynamically
    /// - You want to force a fresh scan of assemblies on the next registration
    /// </remarks>
    public static void ClearRegistrationCaches()
    {
        _assemblyTypesCache.Clear();
        _typeEnvironmentCache.Clear();
        _interfaceNameCache.Clear();
    }

    /// <summary>
    /// Gets the cache statistics for diagnostic purposes.
    /// </summary>
    /// <returns>A dictionary containing cache entry counts.</returns>
    public static Dictionary<string, int> GetCacheStatistics()
    {
        return new Dictionary<string, int>
        {
            ["AssemblyTypes"] = _assemblyTypesCache.Count,
            ["TypeEnvironment"] = _typeEnvironmentCache.Count,
            ["InterfaceName"] = _interfaceNameCache.Count
        };
    }

    /// <summary>
    /// Scans the specified assemblies and automatically registers all classes marked with
    /// <see cref="UsecaseAttribute"/>, <see cref="PresenterAttribute"/>,
    /// <see cref="MiddlewareAttribute"/>, <see cref="PipelineAttribute"/>, or <see cref="ServiceAttribute"/>.
    /// </summary>
    public static IServiceCollection AddCleanArchitectureCore(this IServiceCollection services, params Assembly[] assemblies)
    {
        return AddCleanArchitectureCore(services, environment: null, configureOptions: null, assemblies);
    }

    /// <summary>
    /// Scans the specified assemblies and automatically registers all classes marked with
    /// <see cref="UsecaseAttribute"/>, <see cref="PresenterAttribute"/>,
    /// <see cref="MiddlewareAttribute"/>, <see cref="PipelineAttribute"/>, <see cref="ServiceAttribute"/>, or <see cref="RepositoryAttribute"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure registration options (e.g., enable debug logging).</param>
    /// <param name="assemblies">The assemblies to scan for components. If not specified, the calling assembly is used.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCleanArchitectureCore(this IServiceCollection services, Action<CleanArchitectureOptions> configureOptions, params Assembly[] assemblies)
    {
        return AddCleanArchitectureCore(services, environment: null, configureOptions, assemblies);
    }

    /// <summary>
    /// Scans the specified assemblies and automatically registers all classes marked with
    /// <see cref="UsecaseAttribute"/>, <see cref="PresenterAttribute"/>,
    /// <see cref="MiddlewareAttribute"/>, <see cref="PipelineAttribute"/>, <see cref="ServiceAttribute"/>, or <see cref="RepositoryAttribute"/>.
    /// Filters components based on environment attributes (<see cref="WhenDevAttribute"/>, <see cref="WhenTestAttribute"/>, <see cref="WhenProdAttribute"/>).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="environment">The current environment name (e.g., "Development", "Test", "Production"). Used to filter components marked with environment attributes.</param>
    /// <param name="assemblies">The assemblies to scan for components. If not specified, the calling assembly is used.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCleanArchitectureCore(this IServiceCollection services, string? environment, params Assembly[] assemblies)
    {
        return AddCleanArchitectureCore(services, environment, configureOptions: null, assemblies);
    }

    /// <summary>
    /// Scans the specified assemblies and automatically registers all classes marked with
    /// <see cref="UsecaseAttribute"/>, <see cref="PresenterAttribute"/>,
    /// <see cref="MiddlewareAttribute"/>, <see cref="PipelineAttribute"/>, <see cref="ServiceAttribute"/>, or <see cref="RepositoryAttribute"/>.
    /// Supports multiple implementations per interface and automatic <see cref="IEnumerable{T}"/> resolution.
    /// Filters components based on environment attributes (<see cref="WhenDevAttribute"/>, <see cref="WhenTestAttribute"/>, <see cref="WhenProdAttribute"/>).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="environment">The current environment name (e.g., "Development", "Test", "Production"). Used to filter components marked with environment attributes.</param>
    /// <param name="configureOptions">An optional action to configure registration options. Use this to enable debug logging for troubleshooting registration issues.</param>
    /// <param name="assemblies">The assemblies to scan for components. If not specified, the calling assembly is used.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>Multiple implementations: Services and Repositories can have multiple implementations of the same interface, which can be resolved via <see cref="IEnumerable{T}"/> (Strategy Pattern). Usecases, Presenters, and Pipelines require unique implementations per interface.</para>
    /// <para>Debug mode: Enable debug logging by setting <see cref="CleanArchitectureOptions.Debug"/> to <c>true</c> in the configureOptions action.</para>
    /// </remarks>
    public static IServiceCollection AddCleanArchitectureCore(
        this IServiceCollection services,
        string? environment,
        Action<CleanArchitectureOptions>? configureOptions,
        params Assembly[] assemblies)
    {
        var config = new CleanArchitectureOptions();
        configureOptions?.Invoke(config);

        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        Log(config, $"-> Starting Clean Architecture Scan in environment: '{environment ?? "N/A"}'");

        var errorCollector = new RegistrationErrorCollector();

        foreach (var assembly in assemblies)
        {
            Log(config, $"-> Scanning Assembly: {assembly.GetName().Name}");
            _ = GetAssemblyTypes(assembly);
        }

        foreach (var assembly in assemblies)
        {
            RegisterUsecases(services, assembly, environment, assemblies, config, errorCollector);
            RegisterPresenters(services, assembly, environment, assemblies, config, errorCollector);
            RegisterMiddlewares(services, assembly, environment, assemblies, config, errorCollector);
            RegisterServices(services, assembly, environment, assemblies, config, errorCollector);
            RegisterRepositories(services, assembly, environment, assemblies, config, errorCollector);
            RegisterPipelines(services, assembly, environment, config, errorCollector);
        }

        RegisterDefaultPipeline(services, assemblies, config);

        if (!IsServiceRegistered(services, typeof(IMiddlewareResolver)))
        {
            services.TryAddScoped<IMiddlewareResolver, MiddlewareResolver>();
            Log(config, "-> Registered Internal Service: IMiddlewareResolver -> MiddlewareResolver");
        }

        Log(config, "-> Clean Architecture Scan Completed.");

        // Throw all collected errors at the end (after completion log)
        errorCollector.ThrowIfHasErrors();

        return services;
    }

    private static void RegisterUsecases(IServiceCollection services, Assembly assembly, string? environment, Assembly[] allAssemblies, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        ProcessComponents<UsecaseAttribute>(
            services, assembly, environment, allAssemblies, options, "Usecase",
            typeof(UsecaseBase),
            attr => attr.Implementation,
            attr => attr.AsInterface,
            attr => attr.Lifetime,
            errorCollector
        );
    }

    private static void RegisterPresenters(IServiceCollection services, Assembly assembly, string? environment, Assembly[] allAssemblies, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        ProcessComponents<PresenterAttribute>(
            services, assembly, environment, allAssemblies, options, "Presenter",
            typeof(PresenterBase),
            attr => attr.Implementation,
            attr => attr.AsInterface,
            attr => attr.Lifetime,
            errorCollector
        );
    }

    private static void RegisterServices(IServiceCollection services, Assembly assembly, string? environment, Assembly[] allAssemblies, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        ProcessComponents<ServiceAttribute>(
            services, assembly, environment, allAssemblies, options, "Service",
            baseType: null,
            attr => attr.Implementation,
            attr => attr.AsInterface,
            attr => attr.Lifetime,
            errorCollector
        );
    }

    private static void RegisterRepositories(IServiceCollection services, Assembly assembly, string? environment, Assembly[] allAssemblies, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        ProcessComponents<RepositoryAttribute>(
            services, assembly, environment, allAssemblies, options, "Repository",
            baseType: null,
            attr => attr.Implementation,
            attr => attr.AsInterface,
            attr => attr.Lifetime,
            errorCollector
        );
    }

    private static void RegisterPipelines(IServiceCollection services, Assembly assembly, string? environment, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        var assemblyTypes = GetAssemblyTypes(assembly);

        var pipelineTypes = assemblyTypes
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(IPipeline).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<PipelineAttribute>() != null)
            .Where(t => ShouldRegisterForEnvironment(t, environment))
            .ToList();

        var registrations = new List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)>();

        foreach (var type in pipelineTypes)
        {
            try
            {
                var attribute = type.GetCustomAttribute<PipelineAttribute>()!;
                var implementationType = attribute.Implementation ?? type;
                var serviceType = attribute.AsInterface ?? typeof(IPipeline);
                var lifetime = attribute.Lifetime;

                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    errorCollector.AddError(InvalidOperationException.Create(new Dictionary<string, object>
                    {
                        ["message"] = "pipeline.registration.failed",
                        ["details"] = new Dictionary<string, object>
                        {
                            ["type"] = type.Name,
                            ["implementationType"] = implementationType.Name,
                            ["serviceType"] = serviceType.Name,
                            ["reason"] = $"The class '{type.Name}' must implement the interface '{serviceType.Name}' specified in the attribute or auto-detected.",
                            ["suggestion"] = $"Ensure '{type.Name}' implements '{serviceType.Name}', or remove the interface specification if the class should be registered without an interface."
                        }
                    }));
                    continue;
                }

                registrations.Add((implementationType, serviceType, lifetime, type));
            }
            catch (InvalidOperationException ex)
            {
                errorCollector.AddError(ex);
                continue;
            }
        }

        if (registrations.Count > 0)
        {
            DetectAndReportConflicts(registrations, "Pipeline", environment, errorCollector, out var conflictingInterfaces);
            registrations = [.. registrations.Where(r => !r.serviceType.IsInterface || !conflictingInterfaces.Contains(r.serviceType))];

            foreach (var (implementationType, serviceType, lifetime, originalType) in registrations)
            {
                var descriptor = CreateServiceDescriptor(serviceType, implementationType, lifetime);
                services.TryAdd(descriptor);
                Log(options, $"-> Registered Pipeline: {implementationType.Name} as {serviceType.Name} ({lifetime})");
            }
        }
    }

    private static void ProcessComponents<TAttribute>(
        IServiceCollection services,
        Assembly assembly,
        string? environment,
        Assembly[] allAssemblies,
        CleanArchitectureOptions options,
        string componentKind,
        Type? baseType,
        Func<TAttribute, Type?> getImplementationOverride,
        Func<TAttribute, Type?> getInterfaceOverride,
        Func<TAttribute, ServiceLifetime> getLifetime,
        RegistrationErrorCollector errorCollector)
        where TAttribute : BaseAttribute
    {
        var assemblyTypes = GetAssemblyTypes(assembly);
        var requiresUniqueImplementation = componentKind == "Usecase" || componentKind == "Presenter" || componentKind == "Pipeline";

        var candidates = assemblyTypes
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => baseType == null || t.IsSubclassOf(baseType))
            .Where(t => t.GetCustomAttribute<TAttribute>() != null)
            .Where(t => ShouldRegisterForEnvironment(t, environment))
            .ToList();

        var registrations = new List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)>();

        foreach (var type in candidates)
        {
            try
            {
                var attribute = type.GetCustomAttribute<TAttribute>()!;
                var implementationType = getImplementationOverride(attribute) ?? type;
                var serviceType = getInterfaceOverride(attribute) ?? FindMatchingInterface(type, assembly) ?? type;
                var lifetime = getLifetime(attribute);

                if (getInterfaceOverride(attribute) != null)
                {
                    try
                    {
                        ValidateInterfaceHasImplementation(serviceType, type, allAssemblies, environment, componentKind);
                    }
                    catch (InvalidOperationException ex)
                    {
                        errorCollector.AddError(ex);
                        continue;
                    }
                }

                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    errorCollector.AddError(InvalidOperationException.Create(new Dictionary<string, object>
                    {
                        ["message"] = $"{componentKind.ToLowerInvariant()}.registration.failed",
                        ["details"] = new Dictionary<string, object>
                        {
                            ["componentKind"] = componentKind,
                            ["type"] = type.Name,
                            ["implementationType"] = implementationType.Name,
                            ["serviceType"] = serviceType.Name,
                            ["reason"] = $"The class '{type.Name}' must implement the interface '{serviceType.Name}' specified in the attribute or auto-detected.",
                            ["suggestion"] = $"Ensure '{type.Name}' implements '{serviceType.Name}', or remove the interface specification if the class should be registered without an interface."
                        }
                    }));
                    continue;
                }

                registrations.Add((implementationType, serviceType, lifetime, type));
            }
            catch (InvalidOperationException ex)
            {
                errorCollector.AddError(ex);
                continue;
            }
        }

        if (requiresUniqueImplementation)
        {
            DetectAndReportConflicts(registrations, componentKind, environment, errorCollector, out var conflictingInterfaces);
            registrations = [.. registrations.Where(r => !r.serviceType.IsInterface || !conflictingInterfaces.Contains(r.serviceType))];
        }

        foreach (var (implementationType, serviceType, lifetime, originalType) in registrations)
        {
            var descriptor = CreateServiceDescriptor(serviceType, implementationType, lifetime);

            // Use TryAdd for components that require unique implementations (Usecase, Presenter, Pipeline)
            // Use Add for components that allow multiple implementations (Repository, Service)
            if (requiresUniqueImplementation)
            {
                services.TryAdd(descriptor);
            }
            else
            {
                services.Add(descriptor);
            }

            Log(options, $"-> Registered {componentKind}: {implementationType.Name} as {serviceType.Name} ({lifetime})");
        }
    }

    private static void RegisterMiddlewares(IServiceCollection services, Assembly assembly, string? environment, Assembly[] allAssemblies, CleanArchitectureOptions options, RegistrationErrorCollector errorCollector)
    {
        var preMiddlewareType = typeof(IPreMiddleware);
        var postMiddlewareType = typeof(IPostMiddleware);
        var assemblyTypes = GetAssemblyTypes(assembly);

        var middlewareTypes = assemblyTypes
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => preMiddlewareType.IsAssignableFrom(t) || postMiddlewareType.IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<MiddlewareAttribute>() != null)
            .Where(t => ShouldRegisterForEnvironment(t, environment))
            .ToList();

        foreach (var type in middlewareTypes)
        {
            try
            {
                var attribute = type.GetCustomAttribute<MiddlewareAttribute>()!;
                var implementationType = attribute.Implementation ?? type;
                var serviceType = attribute.AsInterface ?? FindMatchingInterface(type, assembly) ?? type;

                if (attribute.AsInterface != null)
                {
                    try
                    {
                        ValidateInterfaceHasImplementation(attribute.AsInterface, type, allAssemblies, environment, "Middleware");
                    }
                    catch (InvalidOperationException ex)
                    {
                        errorCollector.AddError(ex);
                        continue;
                    }
                }

                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    errorCollector.AddError(InvalidOperationException.Create(new Dictionary<string, object>
                    {
                        ["message"] = "service.registration.failed",
                        ["details"] = new Dictionary<string, object>
                        {
                            ["type"] = type.Name,
                            ["implementationType"] = implementationType.Name,
                            ["serviceType"] = serviceType.Name,
                            ["reason"] = $"The class '{type.Name}' must implement the interface '{serviceType.Name}' specified in the attribute or auto-detected.",
                            ["suggestion"] = $"Ensure '{type.Name}' implements '{serviceType.Name}', or remove the interface specification if the class should be registered without an interface."
                        }
                    }));
                    continue;
                }

                services.TryAdd(CreateServiceDescriptor(serviceType, implementationType, attribute.Lifetime));
                Log(options, $"-> Registered Middleware: {implementationType.Name} as {serviceType.Name}");

                if (serviceType != implementationType && serviceType.IsInterface)
                {
                    var concreteDescriptor = CreateServiceDescriptor(implementationType, implementationType, attribute.Lifetime);
                    services.TryAdd(concreteDescriptor);
                }

                if (preMiddlewareType.IsAssignableFrom(implementationType))
                {
                    services.TryAdd(CreateServiceDescriptor(preMiddlewareType, implementationType, attribute.Lifetime));
                    Log(options, $"   ↳ Hooked as IPreMiddleware");
                }

                if (postMiddlewareType.IsAssignableFrom(implementationType))
                {
                    services.TryAdd(CreateServiceDescriptor(postMiddlewareType, implementationType, attribute.Lifetime));
                    Log(options, $"   ↳ Hooked as IPostMiddleware");
                }
            }
            catch (InvalidOperationException ex)
            {
                errorCollector.AddError(ex);
                continue;
            }
        }
    }

    private static void RegisterDefaultPipeline(IServiceCollection services, Assembly[] assemblies, CleanArchitectureOptions options)
    {
        if (IsServiceRegistered(services, typeof(IPipeline)))
        {
            Log(options, "-> IPipeline already registered, skipping default scan.");
            return;
        }

        Type? pipelineType = null;

        foreach (var assembly in assemblies)
        {
            var assemblyTypes = GetAssemblyTypes(assembly);

            pipelineType = assemblyTypes
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IPipeline).IsAssignableFrom(t))
                .OrderByDescending(t => t.GetCustomAttribute<PipelineAttribute>() != null)
                .FirstOrDefault();

            if (pipelineType != null)
            {
                break;
            }
        }

        if (pipelineType == null)
        {
            var coreAssembly = typeof(IPipeline).Assembly;
            var coreAssemblyTypes = GetAssemblyTypes(coreAssembly);
            pipelineType = coreAssemblyTypes
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IPipeline).IsAssignableFrom(t))
                .OrderByDescending(t => t.GetCustomAttribute<PipelineAttribute>() != null)
                .FirstOrDefault();
        }

        if (pipelineType != null)
        {
            var attribute = pipelineType.GetCustomAttribute<PipelineAttribute>();
            var lifetime = attribute?.Lifetime ?? ServiceLifetime.Scoped;
            var hasAttribute = attribute != null;

            services.TryAddScoped(typeof(IPipeline), pipelineType);

            if (hasAttribute)
            {
                Log(options, $"-> Registered Default Pipeline: {pipelineType.Name} ({lifetime})");
            }
            else
            {
                Log(options, $"-> Registered Default Pipeline: {pipelineType.Name} ({lifetime}) - Consider adding [PipelineAttribute] for explicit registration");
            }
        }
        else
        {
            Log(options, "-> No implementation of IPipeline found. Please create a class implementing IPipeline.");
        }
    }

    private static void Log(CleanArchitectureOptions options, string message)
    {
        if (options.Debug && options.Logger != null)
        {
            options.Logger($"[CleanArchitectureCore] {message}");
        }
    }

    private static string GetAssemblyKey(Assembly assembly)
    {
        if (assembly == null)
        {
            return string.Empty;
        }

        try
        {
            return !string.IsNullOrEmpty(assembly.Location)
                ? $"{assembly.FullName}|{assembly.Location}"
                : assembly.FullName ?? assembly.GetName().Name ?? string.Empty;
        }
        catch
        {
            return assembly.GetName().Name ?? string.Empty;
        }
    }

    private static Type[] GetAssemblyTypes(Assembly assembly)
    {
        var key = GetAssemblyKey(assembly);

        return _assemblyTypesCache.GetOrAdd(key, _ =>
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).ToArray()!;
            }
        });
    }

    private static bool IsServiceRegistered(IServiceCollection services, Type serviceType)
    {
        return services.Any(s => s.ServiceType == serviceType);
    }

    private static ServiceDescriptor CreateServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton(serviceType, implementationType),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped(serviceType, implementationType),
            ServiceLifetime.Transient => ServiceDescriptor.Transient(serviceType, implementationType),
            _ => throw new ArgumentOutOfRangeException(new Dictionary<string, object>
            {
                ["message"] = "invalid.service.lifetime",
                ["details"] = new Dictionary<string, object>
                {
                    ["parameterName"] = nameof(lifetime),
                    ["value"] = lifetime.ToString(),
                    ["reason"] = "Invalid service lifetime."
                }
            })
        };
    }

    private static Type? FindMatchingInterface(Type implementationType, Assembly assembly)
    {
        var interfaceName = "I" + implementationType.Name;
        var assemblyKey = GetAssemblyKey(assembly);

        var interfaceCache = _interfaceNameCache.GetOrAdd(assemblyKey, _ =>
        {
            var types = GetAssemblyTypes(assembly);
            var dict = new Dictionary<string, Type>(StringComparer.Ordinal);
            foreach (var type in types.Where(t => t.IsInterface))
            {
                if (!dict.ContainsKey(type.Name))
                {
                    dict[type.Name] = type;
                }
            }
            return dict;
        });

        if (interfaceCache.TryGetValue(interfaceName, out var foundInterface)
            && implementationType.IsAssignableTo(foundInterface))
        {
            return foundInterface;
        }

        return null;
    }

    private static Type? FindImplementationForInterface(Type interfaceType, Assembly[] assemblies, Type? excludeType = null)
    {
        if (!interfaceType.IsInterface)
        {
            return null;
        }

        foreach (var assembly in assemblies)
        {
            var assemblyTypes = GetAssemblyTypes(assembly);
            var implementation = assemblyTypes
                .FirstOrDefault(t => t.IsClass
                    && !t.IsAbstract
                    && interfaceType.IsAssignableFrom(t)
                    && t != interfaceType
                    && (excludeType == null || t != excludeType));

            if (implementation != null)
            {
                return implementation;
            }
        }

        return null;
    }

    private static void ValidateInterfaceHasImplementation(
        Type? interfaceType,
        Type componentType,
        Assembly[] assemblies,
        string? environment,
        string componentKind)
    {
        if (interfaceType == null || !interfaceType.IsInterface)
        {
            return;
        }

        var componentImplementsInterface = interfaceType.IsAssignableFrom(componentType);

        if (!componentImplementsInterface)
        {
            _ = FindImplementationForInterface(interfaceType, assemblies, componentType) ?? throw InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = $"{componentKind.ToLowerInvariant()}.interface.without.implementation",
                ["details"] = new Dictionary<string, object>
                {
                    ["componentKind"] = componentKind,
                    ["componentType"] = componentType.Name,
                    ["interfaceType"] = interfaceType.Name,
                    ["reason"] = $"The component '{componentType.Name}' does not implement the interface '{interfaceType.Name}' specified in the attribute, and no other implementation exists in the scanned assemblies.",
                    ["environment"] = environment ?? string.Empty,
                    ["suggestion"] = $"Ensure '{componentType.Name}' implements '{interfaceType.Name}' or that another implementation exists in the scanned assemblies."
                }
            });
        }
    }

    /// <summary>
    /// Represents environment attribute combinations using flags to avoid string allocations.
    /// </summary>
    [Flags]
    private enum EnvironmentFlags : byte
    {
        None = 0,
        Dev = 1,
        Test = 2,
        Prod = 4,
        // Combinations
        DevTest = Dev | Test,
        DevProd = Dev | Prod,
        TestProd = Test | Prod,
        All = Dev | Test | Prod
    }

    private sealed class EnvironmentAttributes
    {
        public bool HasWhenDev { get; init; }
        public bool HasWhenTest { get; init; }
        public bool HasWhenProd { get; init; }
        public bool HasNoAttributes { get; init; }

        /// <summary>
        /// Gets the environment flags representation to avoid string allocations.
        /// </summary>
        public EnvironmentFlags GetFlags()
        {
            if (HasNoAttributes)
            {
                return EnvironmentFlags.None; // Special case: no attributes means all environments
            }

            var flags = EnvironmentFlags.None;
            if (HasWhenDev) flags |= EnvironmentFlags.Dev;
            if (HasWhenTest) flags |= EnvironmentFlags.Test;
            if (HasWhenProd) flags |= EnvironmentFlags.Prod;
            return flags;
        }

        public bool OverlapsWith(EnvironmentAttributes other)
        {
            // If either type has no environment attributes, it overlaps with any other type
            if (HasNoAttributes || other.HasNoAttributes)
            {
                return true;
            }

            // Check if they share at least one common environment
            return (HasWhenDev && other.HasWhenDev) ||
                   (HasWhenTest && other.HasWhenTest) ||
                   (HasWhenProd && other.HasWhenProd);
        }
    }

    /// <summary>
    /// Gets environment attributes for a type, with caching to avoid repeated reflection calls.
    /// </summary>
    private static EnvironmentAttributes GetEnvironmentAttributes(Type type)
    {
        var hasWhenDev = type.GetCustomAttribute<WhenDevAttribute>() != null;
        var hasWhenTest = type.GetCustomAttribute<WhenTestAttribute>() != null;
        var hasWhenProd = type.GetCustomAttribute<WhenProdAttribute>() != null;
        var hasNoAttributes = !hasWhenDev && !hasWhenTest && !hasWhenProd;

        return new EnvironmentAttributes
        {
            HasWhenDev = hasWhenDev,
            HasWhenTest = hasWhenTest,
            HasWhenProd = hasWhenProd,
            HasNoAttributes = hasNoAttributes
        };
    }

    /// <summary>
    /// Finds conflicting registrations by checking for overlapping environments.
    /// Optimized approach: Groups registrations by their environment signatures first,
    /// then only compares within groups that could potentially conflict.
    /// This reduces complexity from O(n²) to approximately O(n) in most cases.
    /// </summary>
    private static HashSet<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)>
        FindConflictingRegistrations(
            List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)> registrations)
    {
        if (registrations.Count <= 1)
        {
            return [];
        }

        var conflicts = new HashSet<(Type, Type, ServiceLifetime, Type)>();

        // Cache environment attributes to avoid repeated reflection calls
        var envAttributesCache = new Dictionary<Type, EnvironmentAttributes>(registrations.Count);
        foreach (var (_, _, _, originalType) in registrations)
        {
            if (!envAttributesCache.ContainsKey(originalType))
            {
                envAttributesCache[originalType] = GetEnvironmentAttributes(originalType);
            }
        }

        // Group registrations by their environment flags for more efficient conflict detection
        // Types with no attributes conflict with everything, so handle them separately
        var typesWithNoAttributes = new List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)>();
        var typesByEnvironment = new Dictionary<EnvironmentFlags, List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)>>();

        foreach (var reg in registrations)
        {
            var envAttrs = envAttributesCache[reg.originalType];

            if (envAttrs.HasNoAttributes)
            {
                typesWithNoAttributes.Add(reg);
            }
            else
            {
                // Use environment flags as key to avoid string allocations
                var envFlags = envAttrs.GetFlags();
                if (!typesByEnvironment.TryGetValue(envFlags, out var group))
                {
                    group = [];
                    typesByEnvironment[envFlags] = group;
                }
                group.Add(reg);
            }
        }

        // Types with no attributes conflict with all other types
        if (typesWithNoAttributes.Count > 0)
        {
            foreach (var reg in typesWithNoAttributes)
            {
                conflicts.Add(reg);
            }
            // If there are types with no attributes, they conflict with everything
            if (typesWithNoAttributes.Count > 0 && registrations.Count > typesWithNoAttributes.Count)
            {
                foreach (var reg in registrations)
                {
                    if (!typesWithNoAttributes.Contains(reg))
                    {
                        conflicts.Add(reg);
                    }
                }
            }
        }

        // Check conflicts within each environment group
        foreach (var group in typesByEnvironment.Values)
        {
            if (group.Count > 1)
            {
                // All types in the same environment group conflict with each other
                foreach (var reg in group)
                {
                    conflicts.Add(reg);
                }
            }
        }

        // Check conflicts between different environment groups that overlap
        var envGroups = typesByEnvironment.Values.ToList();
        for (int i = 0; i < envGroups.Count; i++)
        {
            for (int j = i + 1; j < envGroups.Count; j++)
            {
                var group1 = envGroups[i];
                var group2 = envGroups[j];

                // Check if any type from group1 overlaps with any type from group2
                var group1Env = envAttributesCache[group1[0].originalType];
                var group2Env = envAttributesCache[group2[0].originalType];

                if (group1Env.OverlapsWith(group2Env))
                {
                    // All types in both groups conflict
                    foreach (var reg in group1)
                    {
                        conflicts.Add(reg);
                    }
                    foreach (var reg in group2)
                    {
                        conflicts.Add(reg);
                    }
                }
            }
        }

        return conflicts;
    }

    /// <summary>
    /// Builds detailed conflict information including implementation names and environment attributes.
    /// </summary>
    private static (string implementationNames, string detailedInfo)
        BuildConflictDetails(
            List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)> conflicts)
    {
        var implementationNames = string.Join(", ", conflicts.Select(r => r.implementationType.Name));
        var implementationFullNames = conflicts.Select(r => new
        {
            r.implementationType.Name,
            FullName = r.implementationType.FullName ?? r.implementationType.Name,
            HasWhenDev = r.originalType.GetCustomAttribute<WhenDevAttribute>() != null,
            HasWhenTest = r.originalType.GetCustomAttribute<WhenTestAttribute>() != null,
            HasWhenProd = r.originalType.GetCustomAttribute<WhenProdAttribute>() != null
        }).ToList();

        var environmentAttributes = implementationFullNames.Select(impl =>
        {
            var attrs = new List<string>();
            if (impl.HasWhenDev) attrs.Add("[WhenDev]");
            if (impl.HasWhenTest) attrs.Add("[WhenTest]");
            if (impl.HasWhenProd) attrs.Add("[WhenProd]");
            return attrs.Count > 0 ? string.Join(" ", attrs) : "no environment attributes";
        }).ToList();

        var detailedInfo = string.Join("; ", implementationFullNames.Zip(environmentAttributes, (impl, env) =>
            $"{impl.Name} ({env})"));

        return (implementationNames, detailedInfo);
    }

    /// <summary>
    /// Detects and reports conflicts for components that require unique implementations.
    /// Centralizes conflict detection logic to avoid code duplication.
    /// </summary>
    private static void DetectAndReportConflicts(
        List<(Type implementationType, Type serviceType, ServiceLifetime lifetime, Type originalType)> registrations,
        string componentKind,
        string? environment,
        RegistrationErrorCollector errorCollector,
        out HashSet<Type> conflictingInterfaces)
    {
        conflictingInterfaces = [];

        var interfaceGroups = registrations
            .Where(r => r.serviceType.IsInterface)
            .GroupBy(r => r.serviceType)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var conflictGroup in interfaceGroups)
        {
            var interfaceType = conflictGroup.Key;
            var conflictingRegistrations = conflictGroup.ToList();

            // Utiliser HashSet pour meilleure performance
            var actualConflicts = FindConflictingRegistrations(conflictingRegistrations);

            if (actualConflicts.Count == 0)
            {
                continue;
            }

            var conflictsList = actualConflicts.ToList();
            var (implementationNames, detailedInfo) = BuildConflictDetails(conflictsList);

            var messageKey = componentKind == "Pipeline"
                ? "pipeline.multiple.implementations"
                : $"{componentKind.ToLowerInvariant()}.multiple.implementations";

            errorCollector.AddError(InvalidOperationException.Create(new Dictionary<string, object>
            {
                ["message"] = messageKey,
                ["details"] = new Dictionary<string, object>
                {
                    ["componentKind"] = componentKind,
                    ["interfaceType"] = interfaceType.Name,
                    ["interfaceFullName"] = interfaceType.FullName ?? interfaceType.Name,
                    ["implementations"] = implementationNames,
                    ["implementationDetails"] = detailedInfo,
                    ["implementationCount"] = conflictsList.Count,
                    ["environment"] = environment ?? "N/A",
                    ["reason"] = $"Multiple implementations of interface '{interfaceType.Name}' are not allowed for {componentKind} components in the same environment. Found: {implementationNames}",
                    ["suggestion"] = $"To resolve this conflict, you can:\n" +
                                    $"1. Use environment-specific attributes ([WhenDev], [WhenTest], [WhenProd]) to register different implementations for different environments.\n" +
                                    $"2. Specify a unique interface for each implementation using '{componentKind}Attribute' with explicit 'AsInterface' property.\n" +
                                    $"3. Remove one of the conflicting implementations if it's not needed.\n" +
                                    $"4. Combine multiple implementations into a single implementation if appropriate.\n\n" +
                                    $"Conflicting implementations: {detailedInfo}\n" +
                                    $"These implementations have NOT been registered in the DI container due to the conflict."
                }
            }));

            conflictingInterfaces.Add(interfaceType);
        }
    }

    /// <summary>
    /// Determines if a type should be registered for the given environment.
    /// Supports multiple environment attributes on the same class (e.g., [WhenDev] and [WhenProd]).
    /// A class with multiple environment attributes will be registered in all matching environments.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="environment">The environment name (e.g., "Development", "Test", "Production").</param>
    /// <returns>True if the type should be registered for the given environment, false otherwise.</returns>
    /// <remarks>
    /// Examples:
    /// - A class with [WhenDev] and [WhenProd] will be registered in both "Development" and "Production" environments.
    /// - A class with [WhenTest] and [WhenDev] will be registered in both "Test" and "Development" environments.
    /// - A class with no environment attributes will be registered in all environments.
    /// - A class with environment attributes but no matching environment will not be registered.
    /// </remarks>
    private static bool ShouldRegisterForEnvironment(Type type, string? environment)
    {
        var typeFullName = type.FullName ?? type.Name;
        var key = (TypeFullName: typeFullName, Environment: environment);

        return _typeEnvironmentCache.GetOrAdd(key, k =>
        {
            // Check for environment attributes (a class can have multiple attributes)
            var hasWhenDev = type.GetCustomAttribute<WhenDevAttribute>() != null;
            var hasWhenTest = type.GetCustomAttribute<WhenTestAttribute>() != null;
            var hasWhenProd = type.GetCustomAttribute<WhenProdAttribute>() != null;

            // If no environment attributes, register in all environments
            if (!hasWhenDev && !hasWhenTest && !hasWhenProd)
            {
                return true;
            }

            // If environment is not specified, don't register types with environment attributes
            if (string.IsNullOrWhiteSpace(k.Environment))
            {
                return false;
            }

            // Normalize environment name for comparison
            var environmentLower = k.Environment.ToLowerInvariant();
            var isDevelopment = environmentLower == "development" || environmentLower == "dev";
            var isTest = environmentLower == "test";
            var isProduction = environmentLower == "production" || environmentLower == "prod";

            // Register if the environment matches ANY of the attributes on the class
            // This allows a class to be registered in multiple environments by having multiple attributes
            return (hasWhenDev && isDevelopment) || (hasWhenTest && isTest) || (hasWhenProd && isProduction);
        });
    }
}
