using FluentAssertions;

using Modeller.Parsers.Models;

namespace Modeller.ParserTests;

public class EntityKeyParseTests
{
    [Fact]
    public void EntityKeyLine_ShouldMatch()
    {
        const string input = "key TestKey";
        var result = EntityParser.EntityKeySyntax.ParseOrThrow(input);
        result.Name.ToString().Should().Be("TestKey");
        result.Version.HasValue.Should().BeFalse();
    }
    
    [Fact]
    public void EntityKeyParser_UserOwned_CanParser()
    {
        const string fileContent = """
                                   ##############################
                                   #  Determination Close Info  #
                                   ##############################
                                   
                                   key DeterminationCloseInfo : usesOwnerKey(Determination), description("the unique key for a Determination Close Info")
                                   {
                                   }
                                   """;
        var entity = (EntityKeyBuilder)EntityParser.ParseEntityKey(fileContent);
        
        entity.Name.Name.Value.Should().Be("DeterminationCloseInfo");
        entity.Name.Version.HasValue.Should().BeFalse();
        entity.Summary.Value.Should().Be("the unique key for a Determination Close Info");
        entity.Key.Value.Name!.Value.Value.Should().Be("Determination");
        entity.Key.Value.Type.Should().Be("usesOwnerKey");
    }
    
    [Fact]
    public void EntityKeyParser_Untenanted_CanParser()
    {
        const string fileContent = """
                                   ###############################
                                   # IndigenousStatus
                                   ###############################
                                   
                                   key IndigenousStatus : untenanted, description("the unique key for a IndigenousStatus")
                                   {
                                       IndigenousStatusId:
                                           integer(),
                                           description("the IndigenousStatus identifier")
                                   }
                                   """;
        var entity = (EntityKeyBuilder)EntityParser.ParseEntityKey(fileContent);
        entity.Name.Name.Value.Should().Be("IndigenousStatus");
        entity.Name.Version.HasValue.Should().BeFalse();
        entity.Summary.Value.Should().Be("the unique key for a IndigenousStatus");
        entity.Key.Value.Type.Should().Be("untenanted");
        entity.Key.Value.Name.HasValue.Should().BeFalse();
        entity.Fields.Should().HaveCount(1);
        var f =entity.Fields.First();
        f.Name.Value.Should().Be("IndigenousStatusId");
        f.DataType.DataType.Should().Be("integer");
        f.Summary.Value.Should().Be("the IndigenousStatus identifier");
    }
    
    [Fact]
    public void EntityKeyParser_Owner_CanParser()
    {
        const string fileContent = """
                                   ###############################
                                   # OrganisationAddress
                                   ###############################
                                   
                                   key OrganisationAddress : owner(Organisation), description("the unique key for a OrganisationAddress")
                                   {
                                       OrganisationAddressId: 
                                           integer(), 
                                           description("the OrganisationAddress identifier")
                                   }
                                   """;
        var entity = (EntityKeyBuilder)EntityParser.ParseEntityKey(fileContent);
        entity.Name.Name.Value.Should().Be("OrganisationAddress");
        entity.Name.Version.HasValue.Should().BeFalse();
        entity.Summary.Value.Should().Be("the unique key for a OrganisationAddress");
        entity.Key.Value.Name!.Value.Value.Should().Be("Organisation");
        entity.Key.Value.Type.Should().Be("owner");
        entity.Fields.Should().HaveCount(1);
        var f =entity.Fields.First();
        f.Name.Value.Should().Be("OrganisationAddressId");
        f.DataType.DataType.Should().Be("integer");
        f.Summary.Value.Should().Be("the OrganisationAddress identifier");
    }

    [Fact]
    public void EntityKeyParser_Tenant_CanParser()
    {
        const string fileContent = """
                                   ###############################
                                   # Organisation
                                   #
                                   # The organisation is the application Tenant Key
                                   ###############################
                                   
                                   key Organisation : description("the unique key for a Organisation"), tenantkey
                                   {
                                       OrganisationId:
                                           integer(),
                                           description("the Organisation identifier")
                                   }
                                   """;
        var entity = (EntityKeyBuilder)EntityParser.ParseEntityKey(fileContent);
        entity.Name.Name.Value.Should().Be("Organisation");
        entity.Name.Version.HasValue.Should().BeFalse();
        entity.Summary.Value.Should().Be("the unique key for a Organisation");
        entity.Key.HasValue.Should().BeFalse();
        entity.IsTenant.Should().BeTrue();
        entity.Fields.Should().HaveCount(1);
        var f =entity.Fields.First();
        f.Name.Value.Should().Be("OrganisationId");
        f.DataType.DataType.Should().Be("integer");
        f.Summary.Value.Should().Be("the Organisation identifier");
    }}