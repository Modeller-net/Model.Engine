using Domain;
using FluentAssertions;

using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class EntityParseTests
{
    [Fact]
    public void EntityLine_ShouldMatch()
    {
        const string input = "entity TestEntity";
        var result = EntityParser.EntitySyntax.ParseOrThrow(input);
        result.Name.ToString().Should().Be("TestEntity");
        result.Version.HasValue.Should().BeFalse();
    }

    [Fact]
    public void EntityLine_WithVersion_ShouldMatch()
    {
        const string input = "entity dev10.TestEntity";
        var result = EntityParser.EntitySyntax.ParseOrThrow(input);
        result.Name.ToString().Should().Be("TestEntity");
        result.Version.Value.Should().Be("dev10");
    }

    [Fact]
    public void EntityLine_ShouldThrow_WithMissingKeyword()
    {
        const string input = "name TestEntity";
        const string expected = """
                                Parse error.
                                    unexpected n
                                    expected entity keyword
                                    at line 1, col 1
                                """;

        var a = ()=> EntityParser.EntitySyntax.ParseOrThrow(input);
        a.Should().Throw<ParseException>().WithMessage(expected);
    }

    [Fact]
    public void DescriptionLine_ShouldMatch()
    {
        const string input = "description(\"test description\")";

        var result  = EntityParser.DescriptionSyntax.ParseOrThrow(input);
        result.ToString().Should().Be("test description");
    }

    [Fact]
    public void DescriptionLine_ShouldThrow_WithMissingKeyword()
    {
        const string input = "descr(\"test description\")";
        const string expected = """
                                Parse error.
                                    unexpected (
                                    expected 'description("<clear text description>")'
                                    at line 1, col 6
                                """;

        var a = () => EntityParser.DescriptionSyntax.ParseOrThrow(input);
        a.Should().Throw<ParseException>().WithMessage(expected);
    }

    [Fact]
    public void FieldSection_ShouldMatch()
    {
        const string input = """
                             Child:
                               isSingle(entity=Anything),
                               description("The child associated with the booking to bill")
                             """;

        var result = EntityParser.FieldSyntax.ParseOrThrow(input);
        
        result.Name.ToString().Should().Be("Child");
        result.DataType.DataType.Should().Be("isSingle");
        result.DataType.Attributes.Should().HaveCount(1);
        result.DataType.Attributes.First().Value.Value.Should().Be("Anything");
    }
    
    [Fact]
    public void OptionalAttribute_ShouldMatch()
    {
        const string input = """
                             Child:
                               date(optional),
                               description("The child associated with the booking to bill")
                             """;

        var result = EntityParser.FieldSyntax.ParseOrThrow(input);
        
        result.Name.ToString().Should().Be("Child");
        result.DataType.DataType.Should().Be("date");
        result.DataType.Attributes.Should().HaveCount(1);
        result.DataType.Attributes.First().Name.Should().Be("optional");
        result.Summary.Value.Should().Be("The child associated with the booking to bill");
    }
        
    [Fact]
    public void StringAttribute_ShouldMatch()
    {
        const string input = """
                             Child:
                               string(min=1, max=255),
                               description("The child associated with the booking to bill")
                             """;

        var result = EntityParser.FieldSyntax.ParseOrThrow(input);
        
        result.Name.ToString().Should().Be("Child");
        result.DataType.DataType.Should().Be("string");
        result.DataType.Attributes.Should().HaveCount(2);
        result.DataType.Attributes.First().Name.Should().Be("min");
        result.Summary.Value.Should().Be("The child associated with the booking to bill");
    }

    [Fact]
    public void EntityParser_CanParser()
    {
        const string fileContent = """
                                   ##############################
                                   #  Determination Close Info  #
                                   ##############################
                                   
                                   entity dev10.DeterminationCloseInfo : description("fields relating to closing a Determination")
                                   {
                                       Reason:
                                           isSingle(entity=DeterminationCloseReason, entityversion=dev10),
                                           description("The reason why this application was closed.")
                                   
                                       Details:
                                           string(optional, min=1, max=255),
                                           description("Comments")
                                   
                                       WasNotifiedToStateTerritoryBody:
                                           isEnum(enum=DeterminationCloseWasNotifiedToStateTerritoryBody, enumversion=dev10),
                                           description("Has the state/territory body been notified about this closure?")
                                   
                                       DeclaredTrueAndCorrect:
                                           boolean(),
                                           description("Did the user declare the information was true and correct?")
                                   }
                                   """;

        var entity = (EntityBuilder)EntityParser.ParseEntity(fileContent);

        entity.Name.Name.ToString().Should().Be("DeterminationCloseInfo");
        entity.Name.Version.Value.Should().Be("dev10");
        entity.Summary.Value.Should().Be("fields relating to closing a Determination");
        entity.Fields.Should().HaveCount(4);
    }
}