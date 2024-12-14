using FluentAssertions;
using Xunit;

namespace Names.Tests;

public class InflectorExtensionsTests
{
    [Theory]
    [InlineData("cat", "cats")]
    [InlineData("dog", "dogs")]
    [InlineData("person", "people")]
    public void Pluralize_ShouldPluralizeWordsCorrectly(string input, string expected)
    {
        var result = input.Pluralize();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("cats", "cat")]
    [InlineData("dogs", "dog")]
    [InlineData("people", "person")]
    public void Singularize_ShouldSingularizeWordsCorrectly(string input, string expected)
    {
        var result = input.Singularize();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "Hello World")]
    [InlineData("HELLO WORLD", "Hello World")]
    public void Titleize_ShouldTitleizeStringCorrectly(string input, string expected)
    {
        var result = input.Titleize();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("hello-world", "Hello-world")]
    public void Pascalize_ShouldPascalizeStringCorrectly(string input, string expected)
    {
        var result = input.Pascalize();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello_world", "helloWorld")]
    [InlineData("hello-world", "hello-world")]
    public void Camelize_ShouldCamelizeStringCorrectly(string input, string expected)
    {
        var result = input.Camelize();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("helloWorld", "hello_world")]
    public void Underscore_ShouldUnderscoreStringCorrectly(string input, string expected)
    {
        var result = input.Underscore();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello_world", "hello-world")]
    [InlineData("hello world", "hello world")]
    public void Hyphenate_ShouldHyphenateStringCorrectly(string input, string expected)
    {
        var result = input.Hyphenate();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("helloWorld", "hello-world")]
    public void Kebaberize_ShouldKebaberizeStringCorrectly(string input, string expected)
    {
        var result = input.Kebaberize();
        result.Should().Be(expected);
    }
}