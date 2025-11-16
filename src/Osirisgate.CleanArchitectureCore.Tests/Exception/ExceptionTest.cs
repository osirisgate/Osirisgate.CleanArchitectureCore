using Osirisgate.CleanArchitectureCore.Exception;

namespace Osirisgate.CleanArchitectureCore.Tests.Exception;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ExceptionTest
{
    [Fact]
    public void TestExceptionFormatReturnsCorrectStructure()
    {
        // Arrange
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test error message",
            ["details"] = new Dictionary<string, object>
            {
                ["field1"] = "error1",
                ["field2"] = "error2"
            }
        };
        var exception = new BadRequestContentException(errors);

        // Act
        var formatted = exception.Format();

        // Assert
        Assert.NotNull(formatted);
        Assert.Equal("error", formatted["status"]?.ToString());
        Assert.Equal("Test error message", formatted["message"]?.ToString());

        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal("error1", details["field1"]?.ToString());
        Assert.Equal("error2", details["field2"]?.ToString());
    }

    [Fact]
    public void TestExceptionFormatWithEmptyErrors()
    {
        // Arrange
        var exception = new BadRequestContentException(new Dictionary<string, object>());

        // Act
        var formatted = exception.Format();

        // Assert
        Assert.NotNull(formatted);
        Assert.Equal("error", formatted["status"]?.ToString());

        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Empty(details);
    }

    [Fact]
    public void TestExceptionFormatWithNestedDetails()
    {
        // Arrange
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Validation failed",
            ["details"] = new Dictionary<string, object>
            {
                ["user"] = new Dictionary<string, object>
                {
                    ["email"] = "Invalid email format",
                    ["age"] = "Age must be positive"
                }
            }
        };
        var exception = new BadRequestContentException(errors);

        // Act
        var formatted = exception.Format();

        // Assert
        Assert.NotNull(formatted);

        var details = formatted["details"] as IDictionary<string, object>;
        Assert.NotNull(details);

        var userDetails = details["user"] as IDictionary<string, object>;
        Assert.NotNull(userDetails);
        Assert.Equal("Invalid email format", userDetails["email"]?.ToString());
        Assert.Equal("Age must be positive", userDetails["age"]?.ToString());
    }

    [Fact]
    public void TestExceptionGetErrorsReturnsCorrectErrors()
    {
        // Arrange
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = new Dictionary<string, object>()
        };
        var exception = new BadRequestContentException(errors);

        // Act
        var retrievedErrors = exception.GetErrors();

        // Assert
        Assert.NotNull(retrievedErrors);
        Assert.Equal("Test message", retrievedErrors["message"]?.ToString());
    }

    [Fact]
    public void TestExceptionGetDetailsReturnsCorrectDetails()
    {
        // Arrange
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            }
        };
        var exception = new BadRequestContentException(errors);

        // Act
        var details = exception.GetDetails();

        // Assert
        Assert.NotNull(details);
        Assert.Equal("value1", details["key1"]?.ToString());
        Assert.Equal("value2", details["key2"]?.ToString());
    }

    [Fact]
    public void TestExceptionGetDetailsMessageReturnsError()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = new Dictionary<string, object>
            {
                ["error"] = "Error message"
            }
        };
        var exception = new BadRequestContentException(errors);

        var errorMessage = exception.GetDetailsMessage();
        Assert.Equal("Error message", errorMessage);
    }

    [Fact]
    public void TestExceptionGetDetailsMessageReturnsEmptyWhenNoError()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = new Dictionary<string, object>
            {
                ["key1"] = "value1"
            }
        };
        var exception = new BadRequestContentException(errors);

        var errorMessage = exception.GetDetailsMessage();
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void TestExceptionGetDetailsMessageWhenErrorIsNotString()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = new Dictionary<string, object>
            {
                ["error"] = 123
            }
        };
        var exception = new BadRequestContentException(errors);

        var errorMessage = exception.GetDetailsMessage();
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void TestExceptionGetDetailsReturnsEmptyWhenDetailsIsNotDictionary()
    {
        var errors = new Dictionary<string, object>
        {
            ["message"] = "Test message",
            ["details"] = "not a dictionary"
        };
        var exception = new BadRequestContentException(errors);

        var details = exception.GetDetails();
        Assert.NotNull(details);
        Assert.Empty(details);
    }
}
