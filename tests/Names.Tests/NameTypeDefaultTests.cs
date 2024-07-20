using FluentAssertions;

using Xunit;

namespace Names.Tests;

public class NameTypeTests
{
    [Fact]
    public void FromString_NullOrWhitespaceInput_ReturnsError()
    {
        // ReSharper disable once AssignNullToNotNullAttribute
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var a = () => NameType.FromString(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        a.Should().Throw<ArgumentException>().WithMessage("A NameType cannot be null or whitespace. (Parameter 'value')");
    }

    [Fact]
    public void FromString_NonNullNonWhitespaceInput_ReturnsNameType()
    {
        NameType.FromString("test name")
            .Should().BeOfType<NameType>();
    }

    [Fact]
    public void ToPlural_NonNullNonWhitespaceInput_ReturnsNameType()
    {
        NameType.FromString("test name").Value.ToPlural()
            .Should().Be("TestNames");
    }

    [Fact]
    public void ToSingular_NonNullNonWhitespaceInput_ReturnsNameType()
    {
        NameType.FromString("test names").Value.ToSingular()
            .Should().Be("TestName");
    }

    [Fact]
    public void ToString_DisplayFormat_ReturnsString()
    {
        NameType.FromString("test name")
            .ToString("D")
            .Should().Be("Test Name");
    }

    [Fact]
    public void ToString_StaticFormat_ReturnsString()
    {
        NameType.FromString("test name")
            .ToString("S")
            .Should().Be("TestName");
    }

    [Fact]
    public void ToString_LocalFormat_ReturnsString()
    {
        NameType.FromString("test name")
            .ToString("L")
            .Should().Be("testName");
    }

    [Fact]
    public void ToString_ModularFormat_ReturnsString()
    {
        NameType.FromString("test name")
            .ToString("M")
            .Should().Be("_testName");
    }

    [Fact]
    public void FromString_MakeSingleTrue_ReturnsSingularNameType()
    {
        NameType.FromString("names")
            .ToString()
            .Should().Be("Name");
    }

    [Fact]
    public void FromString_MakeSingleFalse_ReturnsPluralNameType()
    {
        NameType.FromString("names", false)
            .ToString()
            .Should().Be("Names");
    }
}