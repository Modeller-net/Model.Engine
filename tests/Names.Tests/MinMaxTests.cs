using System;

using ErrorOr;

using FluentAssertions;

using Xunit;

namespace Names.Tests;

public class MinMaxTests
{
    [Fact]
    public void Set_MinToString_ReturnsValue()
    {
        // Act
        var result = MinMax.Set(5, null);

        // Assert
        result.ToString().Should().Be("5");
    }

    [Fact]
    public void Set_MinMaxToString_ReturnsValue()
    {
        // Act
        var result = MinMax.Set(5, 10);

        // Assert
        result.ToString().Should().Be("5-10");
    }

    [Fact]
    public void Set_MaxToString_ReturnsValue()
    {
        // Act
        var result = MinMax.Set(null, 15);

        // Assert
        result.ToString().Should().Be("15");
    }

    [Fact]
    public void Set_BothMinAndMaxAreNull_ReturnsNull()
    {
        // Act
        var result = MinMax.Set(null, null);

        // Assert
        result.Min().FirstError.Should().Be(Error.NotFound());
        result.Max().FirstError.Should().Be(Error.NotFound());
    }

    [Fact]
    public void Set_BothMinAndMaxHaveValues_ReturnsRange()
    {
        // Act
        var result = MinMax.Set(1, 10);

        // Assert
        result.Min().Value.Should().Be(1);
        result.Max().Value.Should().Be(10);
    }

    [Fact]
    public void Set_OnlyMinHasValue_ReturnsMin()
    {
        // Act
        var result = MinMax.Set(5, null);

        // Assert
        result.Min().Value.Should().Be(5);
        result.Max().FirstError.Should().Be(Error.NotFound());
    }

    [Fact]
    public void Set_OnlyMaxHasValue_ReturnsMax()
    {
        // Act
        var result = MinMax.Set(null, 20);

        // Assert
        result.Min().FirstError.Should().Be(Error.NotFound());
        result.Max().Value.Should().Be(20);
    }

    [Fact]
    public void Set_MinGreaterThanMax_Throws()
    {
        // Act
        var a = () => MinMax.Set(20, 2);

        // Assert
        a.Should().Throw<ArgumentOutOfRangeException>();
    }
}