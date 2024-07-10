using FluentAssertions;
using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class EnumParseTests
{
    [Fact]
    public void EnumValueLine_ShouldMatch()
    {
        const string input = "WAITING:     1, description(\"Waiting for processing\")";
        var result = EntityParser.EnumValueSyntax.ParseOrThrow(input);
        result.Name.Value.Should().Be("WAITING");
        result.Value.Should().Be(1);
        result.Summary.Value.Should().Be("Waiting for processing");
    }
    
    [Fact]
    public void EnumFile_CanParse()
    {
        const string fileContent = """
            ###############################
            # BillingCycle Enum
            ###############################
            enum dev10.BillingCycle : description("Enum representing the billing cycle")
            {
                WEEKLY: 1, description("Weekly")
                FORTNIGHTLY: 2, description("Fortnightly")
                MONTHLY: 4, description("Monthly")
                QUARTERLY: 5, description("Quarterly")
                ANNUALLY: 6, description("Annually")
            }
            """;
        var enumFile = (EnumBuilder)EntityParser.ParseEnum(fileContent);
        enumFile.Name.Name.Value.Should().Be("BillingCycle");
        enumFile.Name.Version.Value.Should().Be("dev10");
        enumFile.Summary.Value.Should().Be("Enum representing the billing cycle");
        enumFile.Enums.Should().HaveCount(5);
    }
}