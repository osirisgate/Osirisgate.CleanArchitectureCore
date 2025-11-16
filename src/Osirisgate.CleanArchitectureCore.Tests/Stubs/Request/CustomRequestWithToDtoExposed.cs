namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithToDtoExposed(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["email"] = true,
        ["age"] = false
    };

    public TDto GetDto<TDto>() where TDto : new()
    {
        return ToDto<TDto>();
    }

    public IDictionary<string, object> GetDtoDictionary()
    {
        return ToDto();
    }
}
