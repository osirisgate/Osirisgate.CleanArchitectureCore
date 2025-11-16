using Microsoft.Extensions.DependencyInjection;
using SystemType = System.Type;

namespace Osirisgate.CleanArchitectureCore.Attributes.Type;

/// <summary>
/// Base class for service registration attributes providing common properties for dependency injection configuration.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public abstract class BaseAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the service lifetime for the component.
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

    /// <summary>
    /// Gets or sets the interface type to register the component as (optional).
    /// If not specified, the component will be registered as its concrete type.
    /// </summary>
    public SystemType? AsInterface { get; set; }

    /// <summary>
    /// Gets or sets the implementation type to register (optional).
    /// Use this when registering an interface with a specific implementation.
    /// </summary>
    public SystemType? Implementation { get; set; }
}
