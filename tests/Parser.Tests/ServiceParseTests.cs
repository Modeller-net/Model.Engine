using FluentAssertions;

using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class ServiceParseTests
{
    [Fact]
    public void ServiceFile_CanParse()
    {
        const string fileContent =
            """
            service OrganisationService : description("the organisation service")
            {
                enums {
                    AddressType
                },
                entities {
                    Country,
                    Gender,
                    Organisation,
                    OrganisationAddress,
                    OrganisationContact,
                    OrganisationSetting,
                    ServiceOffering,
                    State,
                    StructureNode,
                    StructureNodeRule,
                    StructureNodeType,
                    Title,
                    Employee
                },
                references {
                    User: { UserName }
                },
                calls_rpcs {
                    FileStorageRPC
                }
            }
            """;
        var rpcTypeFile = (ServiceBuilder)EntityParser.ParseService(fileContent);
        rpcTypeFile.Name.Name.Value.Should().Be("OrganisationService");
        rpcTypeFile.Summary.Value.Should().Be("the organisation service");
    }

    [Fact]
    public void AuthorisationServiceFile_CanParse()
    {
        const string fileContent =
            """
            service AuthorisationService : description("the authorisation service")
            {
                entities {
                    Right,
                    RightsGroup,
                    Role,
                    SecurityMapping,
                    User
                },
                references {
                    Organisation : { Name },
                    StructureNode : { Name, Description, PreviousName, Parent }
                }
            }
            """;
        var serviceBuilder = (ServiceBuilder)EntityParser.ParseService(fileContent);
        serviceBuilder.Name.Name.Value.Should().Be("AuthorisationService");
        serviceBuilder.Summary.Value.Should().Be("the authorisation service");
    }
}