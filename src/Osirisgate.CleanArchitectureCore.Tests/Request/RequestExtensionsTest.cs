using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Tests.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class RequestExtensionsTest
{
    [Fact]
    public void TestGetRequiredWithExistingFieldReturnsValue()
    {
        var payload = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var request = new CustomRequest(payload);

        var result = request.GetRequired<string>("email");

        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void TestGetRequiredWithMissingFieldThrowsInvalidOperationException()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var exception = Assert.Throws<InvalidOperationException>(() => request.GetRequired<string>("email"));
        var formatted = exception.Format();

        Assert.Contains("Required field 'email' is missing or null", formatted["message"]?.ToString() ?? "");
    }

    [Fact]
    public void TestGetRequiredWithNullRequestThrowsArgumentNullException()
    {
        IRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => request!.GetRequired<string>("email"));
    }

    [Fact]
    public void TestGetRequiredWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var payload = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var request = new CustomRequest(payload);

        Assert.Throws<ArgumentException>(() => request.GetRequired<string>(null!));
        Assert.Throws<ArgumentException>(() => request.GetRequired<string>(""));
        Assert.Throws<ArgumentException>(() => request.GetRequired<string>("   "));
    }

    [Fact]
    public void TestTryGetFieldWithExistingFieldReturnsTrue()
    {
        var payload = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var request = new CustomRequest(payload);

        var result = request.TryGetField<string>("email", out var value);

        Assert.True(result);
        Assert.Equal("test@example.com", value);
    }

    [Fact]
    public void TestTryGetFieldWithMissingFieldReturnsFalse()
    {
        var payload = new Dictionary<string, object>();
        var request = new CustomRequest(payload);

        var result = request.TryGetField<string>("email", out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TestGetOrThrowWithExistingFieldReturnsValue()
    {
        var payload = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var request = new CustomRequest(payload);

        var result = request.GetOrThrow<string>("email");

        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void TestGetOrThrowWithMissingFieldThrowsInvalidOperationException()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            request.GetOrThrow<string>("email"));

        var formatted = exception.Format();
        Assert.Equal("Field 'email' is missing or null.", formatted["message"]?.ToString());
    }

    [Fact]
    public void TestGetRequiredModifiedWithExistingFieldReturnsValue()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);
        request.ModifiedPayload(new Dictionary<string, object> { ["modified"] = "value" });

        var result = request.GetRequiredModified<string>("modified");

        Assert.Equal("value", result);
    }

    [Fact]
    public void TestGetRequiredModifiedWithMissingFieldThrowsInvalidOperationException()
    {
        var payload = new Dictionary<string, object>();
        var request = new CustomRequest(payload);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            request.GetRequiredModified<string>("missing"));

        var formatted = exception.Format();
        Assert.Contains("Required modified field 'missing' is missing or null", formatted["message"]?.ToString() ?? "");
    }

    [Fact]
    public void TestTryGetModifiedFieldWithExistingFieldReturnsTrue()
    {
        var payload = new Dictionary<string, object>();
        var request = new CustomRequest(payload);
        request.ModifiedPayload(new Dictionary<string, object> { ["modified"] = "value" });

        var result = request.TryGetModifiedField<string>("modified", out var value);

        Assert.True(result);
        Assert.Equal("value", value);
    }

    [Fact]
    public void TestTryGetModifiedFieldWithMissingFieldReturnsFalse()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var result = request.TryGetModifiedField<string>("missing", out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TestGetRequiredWithNestedFieldReturnsValue()
    {
        var payload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["email"] = "test@example.com" }
        };
        var request = new CustomRequest(payload);

        var result = request.GetRequired<string>("user.email");

        Assert.Equal("test@example.com", result);
    }
}
