using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Attributes.Type;

/// <summary>
/// Marks a class as a middleware for automatic dependency injection registration.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class MiddlewareAttribute : BaseAttribute
{
    private int _priority = 0;

    /// <summary>
    /// Gets or sets a value indicating whether this middleware is global (applies to all use cases).
    /// If false, the middleware is specific and must be explicitly assigned to use cases.
    /// </summary>
    public bool IsGlobal { get; set; } = false;

    /// <summary>
    /// Gets or sets the execution priority of the middleware. Higher values are executed first.
    /// Default is <c>0</c>. Must be between 0 and 100 (inclusive).
    /// If multiple middlewares have the same priority, the registration order is used.
    /// Global middlewares are always executed before specific middlewares, regardless of their Priority value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when priority is less than 0 or greater than 100.</exception>
    public int Priority
    {
        get => _priority;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(new Dictionary<string, object>
                {
                    ["message"] = "middleware.invalid.priority",
                    ["details"] = new Dictionary<string, object>
                    {
                        ["parameter"] = nameof(Priority),
                        ["value"] = value,
                        ["minimum"] = 0,
                        ["maximum"] = 100,
                        ["reason"] = "Middleware Priority must be between 0 and 100 (inclusive)."
                    }
                });
            }

            _priority = value;
        }
    }
}
