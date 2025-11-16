using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Exception;
using Osirisgate.CleanArchitectureCore.Response;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Tests.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UsecaseTest
{
    [Fact]
    public void TestCanCreateUsecase()
    {
        CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecase();
        Assert.IsType<CustomUsecase>(usecase);
    }

    [Fact]
    public void TestCanNotGetRequestPayloadWithoutSetBefore()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithoutRequestPayload();
            usecase.ExecuteAsync();
        });

        var formatted = exception.Format();
        Assert.Equal("Request is not set. Call WithRequest(request) first.", formatted["message"]?.ToString());
    }

    [Fact]
    public async Task TestCanGetUsecaseException()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "developer@osirisgate.com",
        };

        try
        {
            var request = new RegisterRequestWithPayload(payload);
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithException();
            await usecase
                .WithRequest(request)
                .ExecuteAsync();
        }
        catch (BaseException ex)
        {
            Assert.IsType<UserAlreadyExistsException>(ex);
            Assert.Equal(
                new Dictionary<string, object>
                {
                    ["status"] = Status.Error.GetValue(),
                    ["error_code"] = StatusCode.Conflict.GetValue(),
                    ["message"] = "user.already.exists",
                    ["details"] = payload,
                },
                ex.Format()
            );
        }
    }

    [Fact]
    public void TestCanNotGetRequestFieldWithoutSetBefore()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithoutRequestField();
            usecase.ExecuteAsync();
        });

        var formatted = exception.Format();
        Assert.Equal("Request is not set. Call WithRequest(request) first.", formatted["message"]?.ToString());
    }

    [Fact]
    public async Task TestCanGetUsecaseResponseFromPresenter()
    {
        var presenter = new CustomPresenter();
        CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecase();
        await usecase
            .WithPresenter(presenter)
            .ExecuteAsync();

        var responseFormatted = presenter.GetFormattedResponse();

        Assert.Equal(
            new Dictionary<string, object>
            {
                ["status"] = Status.Success.GetValue(),
                ["code"] = StatusCode.Ok.GetValue(),
                ["message"] = "success.message",
                ["data"] = new Dictionary<string, object>
                {
                    ["key"] = "value",
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }

    [Fact]
    public async Task TestCanGetUsecaseResponseWithRequestFromPresenter()
    {
        var presenter = new CustomPresenter();
        var payload = new Dictionary<string, object>
        {
            ["key"] = "value",
        };
        var request = new CustomRequestWithPayload(payload);

        CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithRequestField();
        await usecase
            .WithRequest(request)
            .WithPresenter(presenter)
            .ExecuteAsync();

        var responseFormatted = presenter.GetFormattedResponse();

        Assert.Equal(
            new Dictionary<string, object>
            {
                ["status"] = Status.Success.GetValue(),
                ["code"] = StatusCode.Ok.GetValue(),
                ["message"] = "success.message",
                ["data"] = new Dictionary<string, object>
                {
                    ["payload"] = payload,
                    ["key"] = "value",
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }

    [Fact]
    public void TestCanThrowErrorWhenNoPresenter()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecase();
            usecase.WithPresenter(null!);
        });

        var formatted = exception.Format();
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("presenter", details["parameter"]?.ToString());
    }

    [Fact]
    public void TestCanThrowErrorWhenNoRequest()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecase();
            usecase.WithRequest(null!);
        });

        var formatted = exception.Format();
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("request", details["parameter"]?.ToString());
    }

    [Fact]
    public void TestCanThrowErrorWhenPresentResponseWithoutPresenter()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithoutPresenter();
            usecase.ExecuteAsync();
        });

        var formatted = exception.Format();
        Assert.Equal("Presenter is not set. Call WithPresenter(presenter) first.", formatted["message"]?.ToString());
    }

    [Fact]
    public async Task TestCanGetModifiedFieldFromRequest()
    {
        var presenter = new CustomPresenter();
        var payload = new Dictionary<string, object>
        {
            ["key"] = "value",
        };
        var request = new CustomRequestWithPayload(payload);
        request.ModifiedPayload(new Dictionary<string, object>
        {
            ["modifiedKey"] = "modifiedValue"
        });

        CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithModifiedField();
        await usecase
            .WithRequest(request)
            .WithPresenter(presenter)
            .ExecuteAsync();

        var responseFormatted = presenter.GetFormattedResponse();
        var data = responseFormatted["data"] as IDictionary<string, object>;

        Assert.NotNull(data);
        Assert.Equal("modifiedValue", data["modifiedValue"]?.ToString());
        Assert.Equal("defaultValue", data["modifiedValueWithDefault"]?.ToString());
    }

    [Fact]
    public async Task TestCanGetModifiedPayloadFromRequest()
    {
        var presenter = new CustomPresenter();
        var payload = new Dictionary<string, object>
        {
            ["key"] = "value",
        };
        var request = new CustomRequestWithPayload(payload);
        request.ModifiedPayload(new Dictionary<string, object>
        {
            ["modifiedKey"] = "modifiedValue"
        });

        CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithModifiedPayload();
        await usecase
            .WithRequest(request)
            .WithPresenter(presenter)
            .ExecuteAsync();

        var responseFormatted = presenter.GetFormattedResponse();
        var data = responseFormatted["data"] as IDictionary<string, object>;

        Assert.NotNull(data);
        var modifiedPayload = data["modifiedPayload"] as IDictionary<string, object>;
        Assert.NotNull(modifiedPayload);
        Assert.Equal("modifiedValue", modifiedPayload["modifiedKey"]?.ToString());
    }

    [Fact]
    public void TestCanThrowErrorWhenGetModifiedFieldWithoutRequest()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithModifiedField();
            usecase.ExecuteAsync();
        });

        var formatted = exception.Format();
        Assert.Equal("Request is not set. Call WithRequest(request) first.", formatted["message"]?.ToString());
    }

    [Fact]
    public void TestCanThrowErrorWhenGetModifiedPayloadWithoutRequest()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            CleanArchitectureCore.Usecase.Usecase usecase = new CustomUsecaseWithModifiedPayload();
            usecase.ExecuteAsync();
        });

        var formatted = exception.Format();
        Assert.Equal("Request is not set. Call WithRequest(request) first.", formatted["message"]?.ToString());
    }
}
