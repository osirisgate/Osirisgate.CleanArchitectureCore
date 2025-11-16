using Osirisgate.CleanArchitectureCore.Attributes.Type;
using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Tests.Attributes;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class MiddlewareAttributeTest
{
    [Fact]
    public void PriorityDefaultValueShouldBeZero()
    {
        // Arrange & Act
        var attribute = new MiddlewareAttribute();

        // Assert
        Assert.Equal(0, attribute.Priority);
    }

    [Fact]
    public void PrioritySetValidValueShouldAcceptValue()
    {
        // Arrange
        var attribute = new MiddlewareAttribute
        {
            // Act
            Priority = 50
        };

        // Assert
        Assert.Equal(50, attribute.Priority);
    }

    [Fact]
    public void PrioritySetMinimumValueShouldAcceptZero()
    {
        // Arrange
        var attribute = new MiddlewareAttribute
        {
            // Act
            Priority = 0
        };

        // Assert
        Assert.Equal(0, attribute.Priority);
    }

    [Fact]
    public void PrioritySetMaximumValueShouldAcceptHundred()
    {
        // Arrange
        var attribute = new MiddlewareAttribute
        {
            // Act
            Priority = 100
        };

        // Assert
        Assert.Equal(100, attribute.Priority);
    }

    [Fact]
    public void PrioritySetNegativeValueShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var attribute = new MiddlewareAttribute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => attribute.Priority = -1);

        Assert.Equal("middleware.invalid.priority", exception.GetErrors()["message"]);
        var details = exception.GetErrors()["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal(nameof(MiddlewareAttribute.Priority), details["parameter"]);
        Assert.Equal(-1, details["value"]);
        Assert.Equal(0, details["minimum"]);
        Assert.Equal(100, details["maximum"]);
    }

    [Fact]
    public void PrioritySetValueGreaterThanHundredShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var attribute = new MiddlewareAttribute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => attribute.Priority = 101);

        Assert.Equal("middleware.invalid.priority", exception.GetErrors()["message"]);
        var details = exception.GetErrors()["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal(nameof(MiddlewareAttribute.Priority), details["parameter"]);
        Assert.Equal(101, details["value"]);
        Assert.Equal(0, details["minimum"]);
        Assert.Equal(100, details["maximum"]);
    }

    [Fact]
    public void PrioritySetLargeValueShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var attribute = new MiddlewareAttribute();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => attribute.Priority = 1000);

        Assert.Equal("middleware.invalid.priority", exception.GetErrors()["message"]);
        var details = exception.GetErrors()["details"] as IDictionary<string, object>;
        Assert.NotNull(details);
        Assert.Equal(1000, details["value"]);
        Assert.Equal(0, details["minimum"]);
        Assert.Equal(100, details["maximum"]);
    }

    [Fact]
    public void PriorityChangeValueShouldUpdateCorrectly()
    {
        // Arrange
        var attribute = new MiddlewareAttribute
        {
            Priority = 50
        };

        // Act
        attribute.Priority = 75;

        // Assert
        Assert.Equal(75, attribute.Priority);
    }

    [Fact]
    public void IsGlobalDefaultValueShouldBeFalse()
    {
        // Arrange & Act
        var attribute = new MiddlewareAttribute();

        // Assert
        Assert.False(attribute.IsGlobal);
    }

    [Fact]
    public void IsGlobal_SetValue_ShouldUpdateCorrectly()
    {
        // Arrange
        var attribute = new MiddlewareAttribute
        {
            // Act
            IsGlobal = true
        };

        // Assert
        Assert.True(attribute.IsGlobal);
    }
}
