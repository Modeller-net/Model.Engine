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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentException>(() => new NonEmptyString(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenValueIsWhitespace()
    {
        Assert.Throws<ArgumentException>(() => new NonEmptyString(" "));
    }

    [Fact]
    public void ShouldConvertToString_WhenImplicitlyConverted()
    {
        var nonEmptyString = new NonEmptyString("Test");

        string value = nonEmptyString;

        Assert.Equal("Test", value);
    }

    [Fact]
    public void ShouldConvertFromString_WhenExplicitlyConverted()
    {
        var value = "Test";

        var nonEmptyString = (NonEmptyString)value;

        Assert.Equal("Test", nonEmptyString.Value);
    }

    [Fact]
    public void ShouldReturnStringValue_WhenToStringIsCalled()
    {
        var nonEmptyString = new NonEmptyString("Test");

        Assert.Equal("Test", nonEmptyString.ToString());
    }
}