namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomRequestWithSuccessfulAsyncValidation(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["email"] = true
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Simulate async operation (e.g., database check, API call)
        await Task.Delay(10, cancellationToken);
        // Validation passes - no exception thrown
    }
}
