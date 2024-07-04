using System;
using Xunit;

namespace Names.Tests;

public class NonEmptyStringTests
{
    [Fact]
    public void ShouldTrimValue_WhenValueHasLeadingOrTrailingWhitespace()
    {
        var nonEmptyString = new NonEmptyString(" Test ");

        Assert.Equal("Test", nonEmptyString.Value);
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenValueIsNull()
    {
        Assert.Throws<ArgumentException>(() => new NonEmptyString(null));
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenValueIsWhitespace()
    {
        Assert.Throws<ArgumentException>(() => new NonEmptyString(" "));
    }

    [Fact]
    public void ShouldConvertToString_WhenImplicitlyConverted()
    {
        NonEmptyString nonEmptyString = new NonEmptyString("Test");

        string value = nonEmptyString;

        Assert.Equal("Test", value);
    }

    [Fact]
    public void ShouldConvertFromString_WhenExplicitlyConverted()
    {
        string value = "Test";

        NonEmptyString nonEmptyString = (NonEmptyString)value;

        Assert.Equal("Test", nonEmptyString.Value);
    }

    [Fact]
    public void ShouldReturnStringValue_WhenToStringIsCalled()
    {
        var nonEmptyString = new NonEmptyString("Test");

        Assert.Equal("Test", nonEmptyString.ToString());
    }
}