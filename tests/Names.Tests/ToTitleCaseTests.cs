using System.Globalization;
using System.Text.RegularExpressions;
using Xunit;

namespace Names.Tests;

public class ToTitleCaseTests
{
    private readonly ToTitleCase _toTitleCase = new ToTitleCase();

    [Fact]
    public void ShouldConvertFirstWordToTitleCase_WhenFirstWordIsTrue()
    {
        var result = _toTitleCase.Transform("test", CultureInfo.CurrentCulture);

        Assert.Equal("Test", result);
    }

    [Fact]
    public void ShouldConvertWordToLowerCase_WhenWordIsArticle()
    {
        var result = _toTitleCase.Transform("a", CultureInfo.CurrentCulture);

        Assert.Equal("A", result);
    }

    [Fact]
    public void ShouldConvertWordToLowerCase_WhenWordIsConjunction()
    {
        var result = _toTitleCase.Transform("and", CultureInfo.CurrentCulture);

        Assert.Equal("And", result);
    }

    [Fact]
    public void ShouldConvertWordToLowerCase_WhenWordIsPreposition()
    {
        var result = _toTitleCase.Transform("at", CultureInfo.CurrentCulture);

        Assert.Equal("At", result);
    }

    [Fact]
    public void ShouldConvertWordToTitleCase_WhenWordIsNotArticleConjunctionOrPreposition()
    {
        var result = _toTitleCase.Transform("test", CultureInfo.CurrentCulture);

        Assert.Equal("Test", result);
    }

    [Fact]
    public void ShouldConvertAllWordsToTitleCase_WhenInputIsSentence()
    {
        var result = _toTitleCase.Transform("this is a test", CultureInfo.CurrentCulture);

        Assert.Equal("This Is a Test", result);
    }

    [Fact]
    public void ShouldConvertAllWordsToTitleCase_WhenInputIsSentenceWithDifferentCasing()
    {
        var result = _toTitleCase.Transform("THIS IS A TEST", CultureInfo.CurrentCulture);

        Assert.Equal("THIS IS A TEST", result);
    }
}