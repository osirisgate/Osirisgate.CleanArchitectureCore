using System.Collections.ObjectModel;
using System.Reflection;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Pipeline;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using ObjectDisposedException = Osirisgate.CleanArchitectureCore.Exception.ObjectDisposedException;

namespace Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;

/// <summary>
/// Resolves and combines global and usecase-specific middlewares for pipeline execution.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class MiddlewareResolver : IMiddlewareResolver
{
    private bool _disposed = false;
    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<IReadOnlyList<Type>> _globalPreMiddlewareTypes;
    private readonly Lazy<IReadOnlyList<Type>> _globalPostMiddlewareTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareResolver"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve middleware instances.</param>
    public MiddlewareResolver(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        _serviceProvider = serviceProvider;
        _globalPreMiddlewareTypes = new Lazy<IReadOnlyList<Type>>(
            GetGlobalPreMiddlewareTypesInternal,
            LazyThreadSafetyMode.ExecutionAndPublication
        );
        _globalPostMiddlewareTypes = new Lazy<IReadOnlyList<Type>>(
            GetGlobalPostMiddlewareTypesInternal,
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    /// <summary>
    /// Gets the pre-middlewares for a specific use case, combining global and usecase-specific middlewares.
    /// </summary>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The combined list of pre-middlewares.</returns>
    /// <exception cref="ArgumentNullException">Thrown if usecaseType is null.</exception>
    public IReadOnlyList<IPreMiddleware> GetPreMiddlewares(Type usecaseType)
    {
        ArgumentNullException.ThrowIfNull(usecaseType, nameof(usecaseType));
        ThrowIfDisposed();

        var global = GetGlobalPreMiddlewares();
        var specific = GetUsecaseSpecificPreMiddlewares(usecaseType);

        return global.Concat(specific).ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the post-middlewares for a specific use case, combining global and usecase-specific middlewares.
    /// </summary>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The combined list of post-middlewares.</returns>
    /// <exception cref="ArgumentNullException">Thrown if usecaseType is null.</exception>
    public IReadOnlyList<IPostMiddleware> GetPostMiddlewares(Type usecaseType)
    {
        ArgumentNullException.ThrowIfNull(usecaseType, nameof(usecaseType));
        ThrowIfDisposed();

        var global = GetGlobalPostMiddlewares();
        var specific = GetUsecaseSpecificPostMiddlewares(usecaseType);

        return global.Concat(specific).ToList().AsReadOnly();
    }

    /// <summary>
    /// Configures a pipeline with middlewares for a specific use case.
    /// </summary>
    /// <param name="pipeline">The pipeline to configure.</param>
    /// <param name="usecaseType">The type of the use case.</param>
    /// <returns>The configured pipeline.</returns>
    /// <exception cref="ArgumentNullException">Thrown if pipeline or usecaseType is null.</exception>
    public IPipeline ConfigurePipeline(IPipeline pipeline, Type usecaseType)
    {
        ArgumentNullException.ThrowIfNull(pipeline, nameof(pipeline));
        ArgumentNullException.ThrowIfNull(usecaseType, nameof(usecaseType));

        ThrowIfDisposed();

        var preMiddlewares = GetPreMiddlewares(usecaseType);
        var postMiddlewares = GetPostMiddlewares(usecaseType);

        pipeline.WithPreMiddlewares(preMiddlewares);
        pipeline.WithPostMiddlewares(postMiddlewares);

        return pipeline;
    }

    private IReadOnlyList<IPreMiddleware> GetGlobalPreMiddlewares()
    {
        ThrowIfDisposed();
        var middlewareTypes = _globalPreMiddlewareTypes.Value;
        var middlewares = new List<IPreMiddleware>();

        foreach (var middlewareType in middlewareTypes)
        {
            if (_serviceProvider.GetService(middlewareType) is IPreMiddleware middleware)
            {
                middlewares.Add(middleware);
            }
        }

        return middlewares.AsReadOnly();
    }

    private ReadOnlyCollection<Type> GetGlobalPreMiddlewareTypesInternal()
    {
        var allPreMiddlewareTypes = new HashSet<Type>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location));

        var preMiddlewareType = typeof(IPreMiddleware);

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && preMiddlewareType.IsAssignableFrom(t))
                    .Where(t => t.GetCustomAttribute<MiddlewareAttribute>() != null)
                    .Where(IsGlobalMiddleware);

                foreach (var type in types)
                {
                    allPreMiddlewareTypes.Add(type);
                }
            }
            catch (ReflectionTypeLoadException)
            {
            }
        }

        var globalMiddlewareTypes = allPreMiddlewareTypes
            .OrderByDescending(GetMiddlewarePriority)
            .ToList();

        return globalMiddlewareTypes.AsReadOnly();
    }

    private IReadOnlyList<IPostMiddleware> GetGlobalPostMiddlewares()
    {
        ThrowIfDisposed();
        var middlewareTypes = _globalPostMiddlewareTypes.Value;
        var middlewares = new List<IPostMiddleware>();

        foreach (var middlewareType in middlewareTypes)
        {
            if (_serviceProvider.GetService(middlewareType) is IPostMiddleware middleware)
            {
                middlewares.Add(middleware);
            }
        }

        return middlewares.AsReadOnly();
    }

    private ReadOnlyCollection<Type> GetGlobalPostMiddlewareTypesInternal()
    {
        var allPostMiddlewareTypes = new HashSet<Type>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location));

        var postMiddlewareType = typeof(IPostMiddleware);

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && postMiddlewareType.IsAssignableFrom(t))
                    .Where(t => t.GetCustomAttribute<MiddlewareAttribute>() != null)
                    .Where(IsGlobalMiddleware);

                foreach (var type in types)
                {
                    allPostMiddlewareTypes.Add(type);
                }
            }
            catch (ReflectionTypeLoadException)
            {
            }
        }

        var globalMiddlewareTypes = allPostMiddlewareTypes
            .OrderByDescending(GetMiddlewarePriority)
            .ToList();

        return globalMiddlewareTypes.AsReadOnly();
    }

    private IReadOnlyList<IPreMiddleware> GetUsecaseSpecificPreMiddlewares(Type usecaseType)
    {
        var attribute = usecaseType.GetCustomAttribute<UsecaseAttribute>();
        if (attribute?.PreMiddlewares == null || attribute.PreMiddlewares.Length == 0)
        {
            return [];
        }

        var middlewares = new List<IPreMiddleware>();
        foreach (var middlewareType in attribute.PreMiddlewares)
        {
            IPreMiddleware? middleware = null;

            if (middlewareType.IsInterface && typeof(IPreMiddleware).IsAssignableFrom(middlewareType))
            {
                middleware = _serviceProvider.GetService(middlewareType) as IPreMiddleware;
            }
            else if (typeof(IPreMiddleware).IsAssignableFrom(middlewareType))
            {
                middleware = _serviceProvider.GetService(middlewareType) as IPreMiddleware;
            }

            if (middleware != null)
            {
                middlewares.Add(middleware);
            }
        }

        return middlewares.OrderByDescending(m => GetMiddlewarePriority(m.GetType())).ToList().AsReadOnly();
    }

    private IReadOnlyList<IPostMiddleware> GetUsecaseSpecificPostMiddlewares(Type usecaseType)
    {
        var attribute = usecaseType.GetCustomAttribute<UsecaseAttribute>();
        if (attribute?.PostMiddlewares == null || attribute.PostMiddlewares.Length == 0)
        {
            return [];
        }

        var middlewares = new List<IPostMiddleware>();
        foreach (var middlewareType in attribute.PostMiddlewares)
        {
            IPostMiddleware? middleware = null;

            if (middlewareType.IsInterface && typeof(IPostMiddleware).IsAssignableFrom(middlewareType))
            {
                middleware = _serviceProvider.GetService(middlewareType) as IPostMiddleware;
            }
            else if (typeof(IPostMiddleware).IsAssignableFrom(middlewareType))
            {
                middleware = _serviceProvider.GetService(middlewareType) as IPostMiddleware;
            }

            if (middleware != null)
            {
                middlewares.Add(middleware);
            }
        }

        return middlewares.OrderByDescending(m => GetMiddlewarePriority(m.GetType())).ToList().AsReadOnly();
    }

    private static bool IsGlobalMiddleware(Type middlewareType)
    {
        var attribute = middlewareType.GetCustomAttribute<MiddlewareAttribute>();
        return attribute?.IsGlobal == true;
    }

    private static int GetMiddlewarePriority(Type middlewareType)
    {
        var attribute = middlewareType.GetCustomAttribute<MiddlewareAttribute>();
        return attribute?.Priority ?? 0;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(MiddlewareResolver));
    }

    /// <summary>
    /// Releases the resources used by the MiddlewareResolver.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
