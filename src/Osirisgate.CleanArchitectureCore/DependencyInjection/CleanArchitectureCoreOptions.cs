namespace Osirisgate.CleanArchitectureCore.DependencyInjection;

/// <summary>
/// Options for configuring Clean Architecture Core registration.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CleanArchitectureOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether debug logging is enabled.
    /// When enabled, detailed information about component registration is logged.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Gets or sets a custom logging action. Defaults to Console.WriteLine when Debug is true.
    /// </summary>
    public Action<string>? Logger { get; set; } = Console.WriteLine;
}
