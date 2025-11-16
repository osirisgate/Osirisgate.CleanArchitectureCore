using Osirisgate.CleanArchitectureCore.Exception;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithApplyingConstraint(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
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

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask;
        throw new BadRequestContentException(errors: new Dictionary<string, object>
        {
            ["message"] = "constraint.applying.failed",
            ["details"] = new Dictionary<string, object>
            {
                ["firstname"] = "required",
                ["lastname"] = "required",
            }
        });
    }
}
