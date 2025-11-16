using SystemType = System.Type;

namespace Osirisgate.CleanArchitectureCore.Attributes.Type;

/// <summary>
/// Marks a class as a use case for automatic dependency injection registration.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class UsecaseAttribute : BaseAttribute
{

    /// <summary>
    /// Gets or sets the types of pre-middlewares to use for this use case (optional).
    /// These middlewares will be combined with global pre-middlewares.
    /// </summary>
    public SystemType[]? PreMiddlewares { get; set; }

    /// <summary>
    /// Gets or sets the types of post-middlewares to use for this use case (optional).
    /// These middlewares will be combined with global post-middlewares.
    /// </summary>
    public SystemType[]? PostMiddlewares { get; set; }
}
