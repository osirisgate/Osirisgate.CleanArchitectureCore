using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomPipelineUsecaseWithRequestField : CleanArchitectureCore.Usecase.Usecase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PresentResponse(CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object>
            {
                ["payload"] = GetPayload(),
                ["modifiedPayload"] = GetModifiedPayload(),
                ["key"] = GetField<string>("Key", string.Empty)!,
            }
        ));
        return Task.CompletedTask;
    }
}

