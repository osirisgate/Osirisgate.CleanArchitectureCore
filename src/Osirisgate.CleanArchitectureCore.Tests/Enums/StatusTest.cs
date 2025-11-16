using Osirisgate.CleanArchitectureCore.Enums;
using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Tests.Enums;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class StatusTest
{
    [Fact]
    public void TestStatusGetValueSuccess()
    {
        var value = Status.Success.GetValue();
        Assert.Equal("success", value);
    }

    [Fact]
    public void TestStatusGetValueError()
    {
        var value = Status.Error.GetValue();
        Assert.Equal("error", value);
    }

    [Fact]
    public void TestStatusGetValueThrowsOnInvalidStatus()
    {
        var invalidStatus = (Status)999;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = invalidStatus.GetValue();
        });

        var formatted = exception.Format();
        Assert.Equal("Unknown status", formatted["message"]?.ToString());
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("999", details["status"]?.ToString());
    }
}
