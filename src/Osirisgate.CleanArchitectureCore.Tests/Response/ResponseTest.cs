using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Response;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ResponseTest
{
    [Fact]
    public void TestCanCreateNewSuccessResponse()
    {
        var data = new Dictionary<string, object>
        {
            ["key"] = "value"
        };

        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: data
        );

        Assert.NotNull(response);
        Assert.True(response.IsSuccess());
        Assert.Equal(StatusCode.Ok.GetValue(), response.GetStatusCode());
        Assert.Equal("success.message", response.GetMessage());
        Assert.Equal(data, response.GetData());

        Assert.Equal("value", response.Get<string>("key"));
        Assert.Null(response.Get<object>("unkown"));

        var expectedOutput = new Dictionary<string, object>
        {
            ["status"] = Status.Success.GetValue(),
            ["code"] = StatusCode.Ok.GetValue(),
            ["message"] = "success.message",
            ["data"] = data,
            ["meta"] = new Dictionary<string, object>()
        };
        Assert.Equal(expectedOutput, response.GetOutput());
    }

    [Fact]
    public void TestCanCreateNewFailedResponse()
    {
        var data = new Dictionary<string, object>
        {
            ["key"] = "value"
        };

        var response = CleanArchitectureCore.Response.Response.Create(
            success: false,
            statusCode: StatusCode.PaymentRequired,
            message: "failed.message",
            data: data
        );

        Assert.NotNull(response);
        Assert.False(response.IsSuccess());
        Assert.Equal(StatusCode.PaymentRequired.GetValue(), response.GetStatusCode());
        Assert.Equal("failed.message", response.GetMessage());
        Assert.Equal(data, response.GetData());

        Assert.Equal("value", response.Get<string>("key"));
        Assert.Null(response.Get<object>("unkown"));

        var expectedOutput = new Dictionary<string, object>
        {
            ["status"] = Status.Error.GetValue(),
            ["code"] = StatusCode.PaymentRequired.GetValue(),
            ["message"] = "failed.message",
            ["details"] = data,
            ["meta"] = new Dictionary<string, object>()
        };
        Assert.Equal(expectedOutput, response.GetOutput());
    }

    [Fact]
    public void TestCanUpdateResponse()
    {
        var data = new Dictionary<string, object>
        {
            ["key"] = "value",
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "ulrich"
            }
        };

        var response = CleanArchitectureCore.Response.Response.Create(
            success: false,
            statusCode: StatusCode.PaymentRequired,
            message: "failed.message",
            data: data
        );

        Assert.NotNull(response);
        Assert.False(response.IsSuccess());
        Assert.Equal(StatusCode.PaymentRequired.GetValue(), response.GetStatusCode());
        Assert.Equal("failed.message", response.GetMessage());
        Assert.Equal(data, response.GetData());
        Assert.Equal("value", response.Get<string>("key"));

        // Update response
        response.SetSuccess(true);
        response.SetMessage("success.message");
        response.SetStatusCode(StatusCode.Ok);
        response.ReplaceData(new Dictionary<string, object>());
        response.UpdateField("key", "updated.value");
        response.UpdateField("user.name", "updated.name");
        response.UpdateField("user.address.city", "updated.city");

        Assert.True(response.IsSuccess());
        Assert.Equal(StatusCode.Ok.GetValue(), response.GetStatusCode());
        Assert.Equal("success.message", response.GetMessage());
        Assert.Equal(new Dictionary<string, object>
        {
            ["key"] = "updated.value",
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "updated.name",
                ["address"] = new Dictionary<string, object>
                {
                    ["city"] = "updated.city"
                }
            }
        }, response.GetData());
        Assert.Equal("updated.value", response.Get<string>("key"));
        Assert.Equal("updated.name", response.Get<string>("user.name"));

        response.ReplaceData(new Dictionary<string, object>());
        Assert.Equal(new Dictionary<string, object>(), response.GetData());
    }

    [Fact]
    public void TestCanUseMetaMethods()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object> { ["key"] = "value" }
        );

        // Initially meta should be empty
        Assert.NotNull(response.GetMeta());
        Assert.Empty(response.GetMeta());

        // Update meta fields
        response.UpdateMetaField("timestamp", DateTime.UtcNow);
        response.UpdateMetaField("requestId", "test-123");
        response.UpdateMetaField("version", "1.0.0");

        // Verify meta fields
        Assert.Equal(3, response.GetMeta().Count);
        Assert.NotEqual(default(DateTime), response.GetMeta<DateTime>("timestamp"));
        Assert.Equal("test-123", response.GetMeta<string>("requestId"));
        Assert.Equal("1.0.0", response.GetMeta<string>("version"));

        // Test nested meta fields
        response.UpdateMetaField("nested.key", "nested.value");
        Assert.Equal("nested.value", response.GetMeta<string>("nested.key"));

        // Test GetMeta with default value
        Assert.Null(response.GetMeta<string>("non.existent"));
        Assert.Equal("default", response.GetMeta<string>("non.existent", "default"));
    }

    [Fact]
    public void TestMetaIsIncludedInOutput()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object> { ["key"] = "value" }
        );

        // Without meta data, output should still include empty meta
        var outputWithoutMeta = response.GetOutput();
        Assert.Contains("meta", outputWithoutMeta.Keys);
        var emptyMeta = Assert.IsType<Dictionary<string, object>>(outputWithoutMeta["meta"]);
        Assert.Empty(emptyMeta);

        // Add meta
        response.UpdateMetaField("timestamp", DateTime.UtcNow);
        response.UpdateMetaField("requestId", "test-123");

        // With meta, output should include meta
        var outputWithMeta = response.GetOutput();
        Assert.Contains("meta", outputWithMeta.Keys);
        var meta = Assert.IsType<Dictionary<string, object>>(outputWithMeta["meta"]);
        Assert.Equal(2, meta.Count);
        Assert.Contains("timestamp", meta.Keys);
        Assert.Contains("requestId", meta.Keys);
    }

    [Fact]
    public void TestCanSetEntireMetaDictionary()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object>()
        );

        var meta = new Dictionary<string, object>
        {
            ["timestamp"] = DateTime.UtcNow,
            ["requestId"] = "test-123",
            ["version"] = "1.0.0",
            ["environment"] = "test"
        };

        response.SetMeta(meta);

        Assert.Equal(4, response.GetMeta().Count);
        Assert.Equal("test-123", response.GetMeta<string>("requestId"));
        Assert.Equal("1.0.0", response.GetMeta<string>("version"));
        Assert.Equal("test", response.GetMeta<string>("environment"));

        // Verify meta is included in output
        var output = response.GetOutput();
        Assert.Contains("meta", output.Keys);
        var outputMeta = Assert.IsType<Dictionary<string, object>>(output["meta"]);
        Assert.Equal(4, outputMeta.Count);
    }

    [Fact]
    public void TestMetaIsAlwaysIncludedEvenWhenEmpty()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "success.message",
            data: new Dictionary<string, object>()
        );

        // Empty meta should still be included
        var output = response.GetOutput();
        Assert.Contains("meta", output.Keys);
        var outputMeta = Assert.IsType<Dictionary<string, object>>(output["meta"]);
        Assert.Empty(outputMeta);

        // Add and then clear meta
        response.UpdateMetaField("key", "value");
        response.SetMeta(new Dictionary<string, object>());

        // Empty meta should still be included
        output = response.GetOutput();
        Assert.Contains("meta", output.Keys);
        outputMeta = Assert.IsType<Dictionary<string, object>>(output["meta"]);
        Assert.Empty(outputMeta);
    }
}
