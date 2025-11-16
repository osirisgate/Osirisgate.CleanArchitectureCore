namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithDeepMissingField(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["user"] = new Dictionary<string, object>()
        {
            ["firstname"] = true,
            ["lastname"] = true,
            ["address"] = new Dictionary<string, object>()
            {
                ["city"] = true,
            }
        }
    };
}
