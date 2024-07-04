using FluentAssertions;

using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class EntityKeyParseTests
{
    [Fact]
    public void EntityKeyLine_ShouldMatch()
    {
        const string input = "key TestKey";
        var result = EntityKeyParser.EntityKeySyntax.ParseOrThrow(input);
        result.Name.ToString().Should().Be("TestKey");
        result.Version.HasValue.Should().BeFalse();
    }
    
    [Fact]
    public void EntityKeyParser_CanParser()
    {
        const string fileContent = """
                                   ##############################
                                   #  Determination Close Info  #
                                   ##############################
                                   
                                   key DeterminationCloseInfo : usesOwnerKey(Determination), description("the unique key for a Determination Close Info")
                                   {
                                   }
                                   """;
        var entity = EntityKeyParser.Parse(fileContent);
    }
}