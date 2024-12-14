using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class ToTests
{
    [Fact]
    public void Transform_ShouldApplyTransformersInOrder()
    {
        var input = "hello world";
        var result = input.Transform(new ToUpperCase(), new ToLowerCase());
        result.Should().Be("hello world");
    }

    [Fact]
    public void Transform_WithCulture_ShouldApplyTransformersInOrder()
    {
        var input = "hello world";
        var culture = new CultureInfo("en-US");
        var result = input.Transform(culture, new ToTitleCase(), new ToLowerCase());
        result.Should().Be("hello world");
    }

    [Fact]
    public void Transform_ShouldReturnInput_WhenNoTransformersProvided()
    {
        var input = "hello world";
        var result = input.Transform();
        result.Should().Be(input);
    }

    [Fact]
    public void Transform_WithCulture_ShouldReturnInput_WhenNoTransformersProvided()
    {
        var input = "hello world";
        var culture = new CultureInfo("en-US");
        var result = input.Transform(culture);
        result.Should().Be(input);
    }
}