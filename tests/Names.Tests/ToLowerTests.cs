using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class ToLowerTests
{
    [Theory]
    [InlineData("HELLO WORLD", "hello world")]
    [InlineData("Hello World", "hello world")]
    [InlineData("H", "h")]
    [InlineData("", "")]
    public void Transform_ShouldConvertToLowerCase(string input, string expected)
    {
        // Arrange
        var transformer = new ToLowerCase();

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().Be(expected);
    }
}