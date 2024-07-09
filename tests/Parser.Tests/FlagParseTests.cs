using FluentAssertions;
using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class FlagParseTests
{
    [Fact]
    public void FlagValueLine_ShouldMatch()
    {
        const string input = "WEDNESDAY: 4, description(\"Wednesday\")";
        var result = EntityParser.FlagValueSyntax.ParseOrThrow(input);
        result.Name.Value.Should().Be("WEDNESDAY");
        result.Value.Should().Be(4);
        result.Summary.Value.Should().Be("Wednesday");
    }
    
    [Fact]
    public void FlagFile_CanParse()
    {
        const string fileContent = """
            ###############################
            # DaysOfWeek Flags
            ###############################
            # IMPORTANT NOTE: the numeric values for a FLAGS definition must be powers of 2
            ###############################
            flags dev10.DaysOfWeek : description("Flags representing a set of days of the week")
            {
                MONDAY: 1, description("Monday")
                TUESDAY: 2, description("Tuesday")
                WEDNESDAY: 4, description("Wednesday")
                THURSDAY: 8, description("Thursday")
                FRIDAY: 16, description("Friday")
                SATURDAY: 32, description("Saturday")
                SUNDAY: 64, description("Sunday")
            }
            """;
        // var flag = EntityParser.ParseFlag(fileContent);
        // flag.Name.Name.Value.Should().Be("DaysOfWeek");
        // flag.Name.Version.Value.Should().Be("dev10");
        // flag.Summary.Value.Should().Be("Flags representing a set of days of the week");
        // flag.Enums.Should().HaveCount(7);
    }
}