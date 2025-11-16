namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithDeepPayload(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["user"] = new Dictionary<string, object>()
        {
            ["name"] = "Jean",
            ["age"] = 18,
            ["address"] = new Dictionary<string, object>()
            {
                ["city"] = "London",
            }
        }
    };
}
