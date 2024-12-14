using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class ToUpperTests
{
    [Theory]
    [InlineData("hello world", "HELLO WORLD")]
    [InlineData("Hello World", "HELLO WORLD")]
    [InlineData("h", "H")]
    [InlineData("", "")]
    public void Transform_ShouldConvertToUpperCase(string input, string expected)
    {
        // Arrange
        var transformer = new ToUpperCase();

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().Be(expected);
    }
}