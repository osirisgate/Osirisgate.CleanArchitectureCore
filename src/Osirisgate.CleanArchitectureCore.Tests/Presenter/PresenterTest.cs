using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Response;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Tests.Presenter;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class PresenterTest
{
    [Fact]
    public void TestCanThrowErrorWhenGetFormattedResponseWithoutResponse()
    {
        var presenter = new CustomPresenter();
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            presenter.GetFormattedResponse();
        });

        var formatted = exception.Format();
        Assert.Equal("Response is not set. Call Present(response) first.", formatted["message"]?.ToString());
    }

    [Fact]
    public void TestCanGetFormattedResponse()
    {
        var presenter = new CustomPresenter();
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object>
            {
                ["key"] = "value"
            }
        );

        presenter.Present(response);
        var formatted = presenter.GetFormattedResponse();

        Assert.Equal(
            new Dictionary<string, object>
            {
                ["status"] = Status.Success.GetValue(),
                ["code"] = StatusCode.Ok.GetValue(),
                ["message"] = "success.message",
                ["data"] = new Dictionary<string, object>
                {
                    ["key"] = "value"
                },
                ["meta"] = new Dictionary<string, object>()
            },
            formatted
        );
    }

    [Fact]
    public void TestCanGetResponse()
    {
        var presenter = new CustomPresenter();
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "test",
            data: null
        );

        presenter.Present(response);
        var retrievedResponse = presenter.GetResponse();

        Assert.NotNull(retrievedResponse);
        Assert.Same(response, retrievedResponse);
    }
}
