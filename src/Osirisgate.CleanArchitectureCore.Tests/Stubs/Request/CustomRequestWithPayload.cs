namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithPayload(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["key"] = true
    };
}
