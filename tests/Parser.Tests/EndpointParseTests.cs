using FluentAssertions;
using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class EndpointParseTests
{
    [Fact]
    public void EndpointFile_CanParse()
    {
        const string fileContent = """
            endpoint dev10.CentreBalanceReport: owner(Account), description("FI-CA balance report for families at a centre.")
            {
                operation("Get"),
                path("account/balance/bycentre"),
                queryparams {
                    CentreToken:
                        type("string"),
                        description("The family to get the balance for.")
            
                    AsAtDate:
                        type("string"),
                        description("The date to get the balance for.")
            
                    IncludeThirdParty:
                        type("bool"),
                        description("Include third party debt when querying FiCa")
                },
                response {
                    CentreFamilyAccountBalanceApiModel:
                        isMultiple(entity=AccountBalance, entityversion=dev10),
                        description("the family account balance api model")
                }
            }
            """;

        var endpoint = (EndpointBuilder)EntityParser.ParseEndpoint(fileContent);
        endpoint.Name.Name.Value.Should().Be("CentreBalanceReport");
        endpoint.Name.Version.Value.Should().Be("dev10");
        endpoint.Summary.Value.Should().Be("FI-CA balance report for families at a centre.");
        endpoint.Owner.Name.Value.Should().Be("Account");
    }
}