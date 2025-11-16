namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithSnakeCaseFields(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["first_name"] = true,
        ["last_name"] = true,
        ["email_address"] = true
    };

    // Public method to expose ToDto<T> for testing
    public TDto GetDto<TDto>() where TDto : new() => ToDto<TDto>();
}
