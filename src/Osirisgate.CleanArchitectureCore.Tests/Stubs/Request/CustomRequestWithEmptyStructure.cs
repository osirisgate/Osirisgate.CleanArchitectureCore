namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithEmptyStructure : CleanArchitectureCore.Request.Request
{
    public CustomRequestWithEmptyStructure(IReadOnlyDictionary<string, object> payload) : base(payload)
    {
    }

    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>();
}
