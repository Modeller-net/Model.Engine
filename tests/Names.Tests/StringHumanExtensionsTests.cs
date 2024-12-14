using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class StringHumanExtensionsTests
{
    [Theory]
    [InlineData("HELLO_WORLD", "HELLO WORLD")]
    [InlineData("HelloWorld", "Hello world")]
    [InlineData("hello_world", "hello world")]
    [InlineData("hello-world", "hello world")]
    [InlineData("hello world", "Hello world")]
    [InlineData("hello _world", "Hello world")]
    [InlineData("hello- world", "Hello world")]
    [InlineData("hello _ world", "Hello world")]
    [InlineData("hello- _world", "Hello world")]
    [InlineData("HELLO WORLD", "Hello world")]
    [InlineData("HELLO WORLD TEST", "Hello world test")]
    [InlineData("HELLO", "HELLO")]
    [InlineData("", "")]
    public void Humanize_ShouldHumanizeStringCorrectly(string input, string expected)
    {
        var result = input.Humanize();
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("HELLO_WORLD", "Title", "HELLO WORLD")]
    [InlineData("HelloWorld", "LowerCase", "hello world")]
    [InlineData("hello_world", "AllCaps", "HELLO WORLD")]
    [InlineData("hello-world", "Sentence", "Hello world")]
    public void Humanize_WithCasing_ShouldHumanizeStringCorrectly(string input, string letterCasing, string expected)
    {
        var casing = Enum.Parse<LetterCasing>(letterCasing); 
        var result = input.Humanize(casing);
        result.Should().Be(expected);
    }
}