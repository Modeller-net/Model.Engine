using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class MinMaxTypeTests
{
    [Fact]
    public void Set_ShouldReturnNotSet_WhenBothMinAndMaxAreNull()
    {
        // Act
        var result = MinMax.Set(null, null);

        // Assert
        result.Should().BeOfType<NotSet>();
    }

    [Fact]
    public void Set_ShouldReturnMin_WhenOnlyMinIsProvided()
    {
        // Act
        var result = MinMax.Set(5, null);

        // Assert
        result.Should().BeOfType<Min>().Which.Value.Should().Be(5);
    }

    [Fact]
    public void Set_ShouldReturnMax_WhenOnlyMaxIsProvided()
    {
        // Act
        var result = MinMax.Set(null, 10);

        // Assert
        result.Should().BeOfType<Max>().Which.Value.Should().Be(10);
    }

    [Fact]
    public void Set_ShouldReturnRange_WhenBothMinAndMaxAreProvided()
    {
        // Act
        var result = MinMax.Set(5, 10);

        // Assert
        result.Should().BeOfType<Range>().Which.Min.Should().Be(5);
        result.Should().BeOfType<Range>().Which.Max.Should().Be(10);
    }

    [Fact]
    public void Min_ShouldReturnError_WhenNotSet()
    {
        // Arrange
        var notSet = new NotSet();

        // Act
        var result = notSet.Min();

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public void Max_ShouldReturnError_WhenNotSet()
    {
        // Arrange
        var notSet = new NotSet();

        // Act
        var result = notSet.Max();

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public void Min_ShouldReturnMinValue_WhenRange()
    {
        // Arrange
        var range = new Range(5, 10);

        // Act
        var result = range.Min();

        // Assert
        result.Value.Should().Be(5);
    }

    [Fact]
    public void Max_ShouldReturnMaxValue_WhenRange()
    {
        // Arrange
        var range = new Range(5, 10);

        // Act
        var result = range.Max();

        // Assert
        result.Value.Should().Be(10);
    }
}