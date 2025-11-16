namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UserRegistrationRequestDto
{
    public string? Email { get; set; }
    public int? Age { get; set; }
    public string? Password { get; set; }
}
