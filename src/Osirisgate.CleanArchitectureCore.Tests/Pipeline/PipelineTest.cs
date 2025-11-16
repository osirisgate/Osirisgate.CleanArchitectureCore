using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Exception;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Response;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Middleware;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Pipeline;

public sealed class PipelineTest
{
    [Fact]
    public async Task TestExecutePipelineWithoutMiddleware()
    {
        var presenter = new CustomPresenter();

        var payload = new Dictionary<string, object>
        {
            ["key"] = "value",
        };
        var request = new CustomRequestWithPayload(payload);

        var usecase = new CustomUsecaseWithRequestField()
            .WithRequest(request)
            .WithPresenter(presenter);

        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline();
        await pipeline.ExecuteAsync(usecase);

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
    public async Task TestPipelineWithPreMiddleware()
    {
        var presenter = new CustomPresenter();

        var payload = new Dictionary<string, object>
        {
            ["key"] = "value",
        };

        var request = new CustomRequestWithPayload(payload);

        var usecase = new CustomPipelineUsecaseWithRequestField()
            .WithRequest(request)
            .WithPresenter(presenter);

        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline()
            .WithPreMiddlewares(new List<IPreMiddleware> { new LogAPreMiddleware(), new LogBPreMiddleware() });

        await pipeline.ExecuteAsync(usecase);

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
                    ["modifiedPayload"] = new Dictionary<string, object>
                    {
                        ["pre_classA"] = nameof(LogAPreMiddleware),
                        ["pre_classB"] = nameof(LogBPreMiddleware),
                    },
                    ["key"] = "value",
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }


    [Fact]
    public async Task TestPipelineWithPostMiddleware()
    {
        var presenter = new CustomPresenter();

        var payload = new Dictionary<string, object>
        {
            ["key"] = "original"
        };

        var request = new CustomRequestWithPayload(payload);

        var usecase = new CustomPipelineUsecaseWithRequestField()
            .WithRequest(request)
            .WithPresenter(presenter);

        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline()
            .WithPostMiddlewares(new List<IPostMiddleware> { new LogBPostMiddleware(), new LogAPostMiddleware() });
        await pipeline.ExecuteAsync(usecase);

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
                    ["modifiedPayload"] = new Dictionary<string, object>(),
                    ["post_classB"] = nameof(LogBPostMiddleware),
                    ["post_classA"] = nameof(LogAPostMiddleware),
                    ["key"] = "original",
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }

    [Fact]
    public async Task TestPipelineWithPreAndPostMiddleware()
    {
        var presenter = new CustomPresenter();

        var payload = new Dictionary<string, object>
        {
            ["key"] = "original"
        };

        var request = new CustomRequestWithPayload(payload);

        var usecase = new CustomPipelineUsecaseWithRequestField()
            .WithRequest(request)
            .WithPresenter(presenter);

        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline()
            .WithPreMiddlewares(new List<IPreMiddleware> { new LogAPreMiddleware(), new LogBPreMiddleware() })
            .WithPostMiddlewares(new List<IPostMiddleware> { new LogBPostMiddleware(), new LogAPostMiddleware() });
        await pipeline.ExecuteAsync(usecase);

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
                    ["modifiedPayload"] = new Dictionary<string, object>
                    {
                        ["pre_classA"] = nameof(LogAPreMiddleware),
                        ["pre_classB"] = nameof(LogBPreMiddleware),
                    },
                    ["post_classB"] = nameof(LogBPostMiddleware),
                    ["post_classA"] = nameof(LogAPostMiddleware),
                    ["key"] = "original",
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }

    [Fact]
    public async Task TestPipelineWitMiddlewareWithoutRunningUsecase()
    {
        var presenter = new CustomPresenter();

        var payload = new Dictionary<string, object>
        {
            ["key"] = "original"
        };

        var request = new CustomRequestWithPayload(payload);

        var usecase = new CustomPipelineUsecaseWithRequestField()
            .WithRequest(request)
            .WithPresenter(presenter);

        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline()
            .WithPreMiddlewares(new List<IPreMiddleware>
                { new LogAPreMiddleware(), new StopPreMiddlewareWithResponse() });
        await pipeline.ExecuteAsync(usecase);

        var responseFormatted = presenter.GetFormattedResponse();

        Assert.Equal(new Dictionary<string, object>
        {
            ["pre_classA"] = nameof(LogAPreMiddleware)
        }, request.GetModifiedPayload());

        Assert.Equal(
            new Dictionary<string, object>
            {
                ["status"] = Status.Success.GetValue(),
                ["code"] = StatusCode.Ok.GetValue(),
                ["message"] = "success.message",
                ["data"] = new Dictionary<string, object>
                {
                    ["stop_pre_with_response"] = nameof(StopPreMiddlewareWithResponse),
                },
                ["meta"] = new Dictionary<string, object>()
            },
            responseFormatted
        );
    }

    [Fact]
    public async Task TestPipelineWitMiddlewaresException()
    {
        try
        {
            var presenter = new CustomPresenter();

            var payload = new Dictionary<string, object>
            {
                ["key"] = "original"
            };

            var request = new CustomRequestWithPayload(payload);

            var usecase = new CustomPipelineUsecaseWithRequestField()
                .WithRequest(request)
                .WithPresenter(presenter);

            var pipeline = new CleanArchitectureCore.Pipeline.Pipeline()
                .WithPreMiddlewares(new List<IPreMiddleware>
                    { new StopPreMiddlewareWithException(), new LogAPreMiddleware() });
            await pipeline.ExecuteAsync(usecase);
        }
        catch (BaseException ex)
        {
            Assert.IsType<BadRequestContentException>(ex);
            Assert.Equal(
                new Dictionary<string, object>
                {
                    ["status"] = Status.Error.GetValue(),
                    ["error_code"] = StatusCode.BadRequest.GetValue(),
                    ["message"] = "stop.pre.without.response",
                    ["details"] = new Dictionary<string, object>
                    {
                        ["class"] = nameof(StopPreMiddlewareWithException),
                    }
                },
                ex.Format()
            );
        }
    }
}
