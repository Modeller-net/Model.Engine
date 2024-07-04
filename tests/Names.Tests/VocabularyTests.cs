using Xunit;

namespace Names.Tests;

public class VocabularyTests
{
    private readonly Vocabulary _vocabulary = new Vocabulary();

    [Fact]
    public void ShouldAddIrregularWordsCorrectly()
    {
        _vocabulary.AddIrregular("person", "people");

        Assert.Equal("people", _vocabulary.Pluralize("person"));
        Assert.Equal("person", _vocabulary.Singularize("people"));
    }

    [Fact]
    public void ShouldAddUncountableWordsCorrectly()
    {
        _vocabulary.AddUncountable("fish");

        Assert.Equal("fish", _vocabulary.Pluralize("fish"));
        Assert.Equal("fish", _vocabulary.Singularize("fish"));
    }

    [Fact]
    public void ShouldAddPluralizationRuleCorrectly()
    {
        _vocabulary.AddPlural("(bus)$", "$1es");

        Assert.Equal("buses", _vocabulary.Pluralize("bus"));
    }

    [Fact]
    public void ShouldAddSingularizationRuleCorrectly()
    {
        _vocabulary.AddSingular("(vert|ind)ices$", "$1ex");

        Assert.Equal("vertex", _vocabulary.Singularize("vertices"));
        Assert.Equal("index", _vocabulary.Singularize("indices"));
    }

    [Fact]
    public void ShouldPluralizeCorrectly()
    {
        _vocabulary.AddPlural("(bus)$", "$1es");

        Assert.Equal("buses", _vocabulary.Pluralize("bus"));
    }

    [Fact]
    public void ShouldSingularizeCorrectly()
    {
        _vocabulary.AddSingular("(vert|ind)ices$", "$1ex");

        Assert.Equal("vertex", _vocabulary.Singularize("vertices"));
    }

    [Fact]
    public void Pluralize_ShouldReturnSameWord_WhenInputIsKnownToBePluralIsFalseAndWordIsUncountable()
    {
        var result = _vocabulary.Pluralize("fish", false);

        Assert.Equal("fish", result);
    }

    [Fact]
    public void Pluralize_ShouldReturnSameWord_WhenInputIsKnownToBePluralIsFalseAndWordIsAlreadyPlural()
    {
        var result = _vocabulary.Pluralize("buses", false);

        Assert.Equal("buses", result);
    }
}