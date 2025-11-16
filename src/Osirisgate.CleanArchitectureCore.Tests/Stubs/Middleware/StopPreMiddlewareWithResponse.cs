using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class StopPreMiddlewareWithResponse : IPreMiddleware
{
    public async Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (presenter != null)
        {
            var response = presenter.GetResponse();
            if (response == null)
            {
                response = CleanArchitectureCore.Response.Response.Create(
                    success: true,
                    statusCode: StatusCode.Ok,
                    message: "success.message",
                    data: new Dictionary<string, object>
                    {
                        ["stop_pre_with_response"] = nameof(StopPreMiddlewareWithResponse)
                    }
                );
                presenter.Present(response);
            }
            else
            {
                response.UpdateField("stop_pre_with_response", nameof(StopPreMiddlewareWithResponse));
            }
        }
        else
        {
            await next(cancellationToken);
        }
    }
}
