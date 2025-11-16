namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class TestRequestForDtoMappingOptional(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    // Empty structure allows any fields for flexible testing
    protected override Dictionary<string, object> RequestStructure => [];

    // Public method to expose ToDto<T> for testing
    public TDto GetDto<TDto>() where TDto : new() => ToDto<TDto>();

    // Public method to expose ToDto() for testing
    public IDictionary<string, object> GetDtoDictionary() => ToDto();
}

