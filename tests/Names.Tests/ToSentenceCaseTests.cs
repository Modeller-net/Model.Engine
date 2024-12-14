using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class ToSentenceCaseTests
{
    [Theory]
    [InlineData("hello world", "Hello world")]
    [InlineData("Hello World", "Hello World")]
    [InlineData("h", "H")]
    [InlineData("", "")]
    public void Transform_ShouldConvertToSentenceCase(string input, string expected)
    {
        // Arrange
        var transformer = new ToSentenceCase();

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "Hello world", "en-US")]
    [InlineData("hello world", "Hello world", "tr-TR")]
    public void Transform_WithCulture_ShouldConvertToSentenceCase(string input, string expected, string cultureName)
    {
        // Arrange
        var transformer = new ToSentenceCase();
        var culture = new CultureInfo(cultureName);

        // Act
        var result = transformer.Transform(input, culture);

        // Assert
        result.Should().Be(expected);
    }
}