namespace Osirisgate.CleanArchitectureCore.Attributes.Environment;

/// <summary>
/// Marks a class to be registered only in Test environment.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class WhenTestAttribute : Attribute
{
}
