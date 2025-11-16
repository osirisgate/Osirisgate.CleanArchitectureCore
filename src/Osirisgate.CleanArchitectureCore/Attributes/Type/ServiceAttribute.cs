namespace Osirisgate.CleanArchitectureCore.Attributes.Type;

/// <summary>
/// Marks a class as a service or repository for automatic dependency injection registration.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ServiceAttribute : BaseAttribute
{
}
