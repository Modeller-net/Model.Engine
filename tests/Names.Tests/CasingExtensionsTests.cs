using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class CasingExtensionsTests
{
    [Theory]
    [InlineData("hello world", "Title", "Hello World")]
    [InlineData("hello world", "LowerCase", "hello world")]
    [InlineData("hello world", "AllCaps", "HELLO WORLD")]
    [InlineData("hello world", "Sentence", "Hello world")]
    public void ApplyCase_ShouldChangeCasing(string input, string casing, string expected)
    {
        var letterCase = Enum.Parse<LetterCasing>(casing);
        
        // Act
        var result = input.ApplyCase(letterCase);

        // Assert
        result.Should().Be(expected);
    }
}