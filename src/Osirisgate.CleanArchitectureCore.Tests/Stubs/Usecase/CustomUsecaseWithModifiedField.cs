using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomUsecaseWithModifiedField : CleanArchitectureCore.Usecase.Usecase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var modifiedValue = GetModifiedField<string>("modifiedKey") ?? "null";
        var modifiedValueWithDefault = GetModifiedField<string>("nonExistentKey", "defaultValue");

        PresentResponse(CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object>
            {
                ["modifiedValue"] = modifiedValue,
                ["modifiedValueWithDefault"] = modifiedValueWithDefault!
            }
        ));
        return Task.CompletedTask;
    }
}
