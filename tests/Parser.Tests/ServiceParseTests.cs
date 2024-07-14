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
        var serviceBuilder = (ServiceBuilder)EntityParser.ParseService(fileContent);
        serviceBuilder.Name.Name.Value.Should().Be("OrganisationService");
        serviceBuilder.Summary.Value.Should().Be("the organisation service");
        serviceBuilder.Content.Entities.Items.Should().HaveCount(13);
        serviceBuilder.Content.References.Items.Should().HaveCount(1);
        serviceBuilder.Content.Enums.Items.Should().HaveCount(1);
        serviceBuilder.Content.CallsRpcs.Items.Should().HaveCount(1);
        serviceBuilder.Content.ImplementsRpcs.Should().BeNull();
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
        serviceBuilder.Content.Entities.Items.Should().HaveCount(5);
        serviceBuilder.Content.References.Items.Should().HaveCount(2);
    }

    [Fact]
    public void ReferencesService_CanParse()
    {
        const string fileContent =
            """
            references {
                Organisation : { Name },
                StructureNode : { Name, Description, PreviousName, Parent }
            }
            """;
        var serviceImplementsRpcs = EntityParser.ReferencesSyntax.ParseOrThrow(fileContent);
        serviceImplementsRpcs.Items.Should().HaveCount(2);
        serviceImplementsRpcs.Items.First().Name.Value.Should().Be("Organisation");
    }

    [Fact]
    public void ItemsService_CanParse()
    {
        const string fileContent =
            """
            implements_rpcs {
                FileStorageRPC
            }
            """;
        var serviceImplementsRpcs = EntityParser.ImplementsRpcsSyntax.ParseOrThrow(fileContent);
        serviceImplementsRpcs.Items.Should().HaveCount(1);
        serviceImplementsRpcs.Items.First().Value.Should().Be("FileStorageRPC");
    }
    
    [Fact]
    public void DocumentStorageServiceFile_CanParse()
    {
        const string fileContent =
            """
            service DocumentStorageService : description("the document storage service")
            {
                implements_rpcs {
                    FileStorageRPC
                }
            }
            """;
        var serviceBuilder = (ServiceBuilder)EntityParser.ParseService(fileContent);
        serviceBuilder.Name.Name.Value.Should().Be("DocumentStorageService");
        serviceBuilder.Summary.Value.Should().Be("the document storage service");
        serviceBuilder.Content.ImplementsRpcs.Items.Should().HaveCount(1);
        serviceBuilder.Content.ImplementsRpcs.Items.First().Value.Should().Be("FileStorageRPC");
    }
}