using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Enums;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public enum Status
{
    Success = 1,
    Error = 2,
}

/// <summary>
/// Extension methods for <see cref="Status"/> enum.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class StatusExtensions
{
    /// <summary>
    /// Gets the string value of the status.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns>The string value of the status.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the status is unknown.</exception>
    public static string GetValue(this Status status) => status switch
    {
        Status.Success => "success",
        Status.Error => "error",
        _ => throw new ArgumentOutOfRangeException(new Dictionary<string, object>
        {
            ["message"] = "Unknown status",
            ["details"] = new Dictionary<string, object>
            {
                ["status"] = status.ToString()
            }
        })
    };
}
