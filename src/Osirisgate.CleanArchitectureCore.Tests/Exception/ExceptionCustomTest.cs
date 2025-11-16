using Osirisgate.CleanArchitectureCore.Exception;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using InvalidCastException = Osirisgate.CleanArchitectureCore.Exception.InvalidCastException;
using ObjectDisposedException = Osirisgate.CleanArchitectureCore.Exception.ObjectDisposedException;

namespace Osirisgate.CleanArchitectureCore.Tests.Exception;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ExceptionCustomTest
{
    [Fact]
    public void TestArgumentExceptionCreateWithMessageFormatsCorrectly()
    {
        var exception = ArgumentException.Create("Invalid argument", "paramName");

        var formatted = exception.Format();
        Assert.Equal("Invalid argument", formatted["message"]?.ToString());
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("paramName", details["parameter"]?.ToString());
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrEmptyWithNullThrows()
    {
        Assert.Throws<ArgumentException>(() => ArgumentException.ThrowIfNullOrEmpty(null, "param"));
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrEmptyWithEmptyThrows()
    {
        Assert.Throws<ArgumentException>(() => ArgumentException.ThrowIfNullOrEmpty("", "param"));
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrEmptyWithValidDoesNotThrow()
    {
        ArgumentException.ThrowIfNullOrEmpty("value", "param");
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrWhiteSpaceWithNullThrows()
    {
        Assert.Throws<ArgumentException>(() => ArgumentException.ThrowIfNullOrWhiteSpace(null, "param"));
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrWhiteSpaceWithEmptyThrows()
    {
        Assert.Throws<ArgumentException>(() => ArgumentException.ThrowIfNullOrWhiteSpace("", "param"));
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrWhiteSpaceWithWhitespaceThrows()
    {
        Assert.Throws<ArgumentException>(() => ArgumentException.ThrowIfNullOrWhiteSpace("   ", "param"));
    }

    [Fact]
    public void TestArgumentExceptionThrowIfNullOrWhiteSpaceWithValidDoesNotThrow()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("value", "param");
    }

    [Fact]
    public void TestArgumentExceptionCreateWithErrorsDictionary()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Custom error message",
            ["details"] = new Dictionary<string, object>
            {
                ["parameter"] = "testParam",
                ["reason"] = "Custom reason"
            }
        };
        var exception = ArgumentException.Create(errors);

        var formatted = exception.Format();
        Assert.Equal("Custom error message", formatted["message"]?.ToString());
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("testParam", details["parameter"]?.ToString());
        Assert.Equal("Custom reason", details["reason"]?.ToString());
    }

    [Fact]
    public void TestInvalidCastExceptionCreateFormatsCorrectly()
    {
        var exception = InvalidCastException.Create("System.String", "System.Int32", "test");

        var formatted = exception.Format();
        Assert.Contains("Cannot cast System.String to System.Int32", formatted["message"]?.ToString() ?? "");
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("System.String", details["source_type"]?.ToString());
        Assert.Equal("System.Int32", details["target_type"]?.ToString());
        Assert.Equal("test", details["value"]?.ToString());
    }

    [Fact]
    public void TestInvalidCastExceptionCreateWithNullValueFormatsCorrectly()
    {
        var exception = InvalidCastException.Create("System.String", "System.Int32");

        var formatted = exception.Format();
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("null", details["value"]?.ToString());
    }

    [Fact]
    public void TestObjectDisposedExceptionCreateFormatsCorrectly()
    {
        var exception = ObjectDisposedException.Create("TestObject");

        var formatted = exception.Format();
        Assert.Contains("Cannot access a disposed object", formatted["message"]?.ToString() ?? "");
        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("TestObject", details["object_name"]?.ToString());
    }

    [Fact]
    public void TestObjectDisposedExceptionThrowIfWithDisposedThrows()
    {
        Assert.Throws<ObjectDisposedException>(() => ObjectDisposedException.ThrowIf(true, "TestObject"));
    }

    [Fact]
    public void TestObjectDisposedExceptionThrowIfWithNotDisposedDoesNotThrow()
    {
        ObjectDisposedException.ThrowIf(false, "TestObject");
    }

    [Fact]
    public void TestRuntimeExceptionCreateAndFormat()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Runtime error occurred",
            ["details"] = new Dictionary<string, object>
            {
                ["error"] = "A runtime error occurred"
            }
        };
        var exception = new RuntimeException(errors);

        var formatted = exception.Format();
        Assert.NotNull(formatted);
        Assert.Equal("error", formatted["status"]?.ToString());
        Assert.Equal(500, formatted["error_code"]);
        Assert.Equal("Runtime error occurred", formatted["message"]?.ToString());

        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("A runtime error occurred", details["error"]?.ToString());
    }

    [Fact]
    public void TestRuntimeExceptionGetErrorCode()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Runtime error"
        };
        var exception = new RuntimeException(errors);

        Assert.Equal(500, exception.GetErrorCode());
    }

    [Fact]
    public void TestInvalidCastExceptionCreateWithErrorsDictionary()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Custom cast error",
            ["details"] = new Dictionary<string, object>
            {
                ["source_type"] = "String",
                ["target_type"] = "Int"
            }
        };
        var exception = InvalidCastException.Create(errors);

        var formatted = exception.Format();
        Assert.Equal("Custom cast error", formatted["message"]?.ToString());
    }

    [Fact]
    public void TestObjectDisposedExceptionCreateWithErrorsDictionary()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Object disposed error",
            ["details"] = new Dictionary<string, object>
            {
                ["object_name"] = "TestObject"
            }
        };
        var exception = ObjectDisposedException.Create(errors);

        var formatted = exception.Format();
        Assert.Equal("Object disposed error", formatted["message"]?.ToString());
    }
}
