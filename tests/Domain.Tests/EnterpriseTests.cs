using FluentAssertions;
using Names;

namespace Domain.Tests;

public class EnterpriseExtensionsTests
{
    [Fact]
    public void GetEntitiesByService_ShouldReturnEntitiesForService()
    {
        var service = NameType.FromString("Service1");
        var entity1 = Entity.Create(NameType.FromString("Entity1"), new NonEmptyString("Summary"), []);
        var entity2 = Entity.Create(NameType.FromString("Entity2"), new NonEmptyString("Summary"), []);
        var enterprise = new Enterprise("Company", service, new NonEmptyString("Summary"))
        {
            Services = new List<Service>
            {
                new Service(service, new NonEmptyString("Summary"))
                    { Entities = new List<NameType> { entity1.Name, entity2.Name } }
            },
            Entities = new List<EntityType> { entity1, entity2 }
        };

        var result = enterprise.GetEntitiesByService(service).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(entity1);
        result.Should().Contain(entity2);
    }

    [Fact]
    public void GetEntitiesByService_ShouldReturnEntitiesForServiceWithPredicate()
    {
        var service = NameType.FromString("Service1");
        var entity1 = Entity.Create(NameType.FromString("Entity1"), new NonEmptyString("Summary"), []);
        var entity2 = Entity.Create(NameType.FromString("Entity2"), new NonEmptyString("Summary"), []);
        var enterprise = new Enterprise("Company", service, new NonEmptyString("Summary"))
        {
            Services = new List<Service>
            {
                new Service(service, new NonEmptyString("Summary"))
                    { Entities = new List<NameType> { entity1.Name, entity2.Name } }
            },
            Entities = new List<EntityType> { entity1, entity2 }
        };

        var result = enterprise.GetEntitiesByService(service, e => e.Name == entity1.Name).ToList();

        result.Should().HaveCount(1);
        result.Should().Contain(entity1);
    }

    [Fact]
    public void GetEntitiesByService_ShouldReturnEntitiesForServiceWithoutReferences()
    {
        var service = NameType.FromString("Service1");
        var entity1 = Entity.Create(NameType.FromString("Entity1"), new NonEmptyString("Summary"), []);
        var entity2 = Entity.Create(NameType.FromString("Entity2"), new NonEmptyString("Summary"), []);
        var enterprise = new Enterprise("Company", service, new NonEmptyString("Summary"))
        {
            Services = new List<Service>
            {
                new Service(service, new NonEmptyString("Summary"))
                {
                    Entities = new List<NameType> { entity1.Name },
                    References = new List<ReferenceEntity> { new(entity2.Name) }
                }
            },
            Entities = new List<EntityType> { entity1, entity2 }
        };

        var result = enterprise.GetEntitiesByService(service, includeReferences: false).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be(entity1.Name);
    }

    [Fact]
    public void GetEntitiesByService_ShouldReturnEntitiesForServiceWithReferences()
    {
        var service = NameType.FromString("Service1");
        var entity1 = Entity.Create(NameType.FromString("Entity1"), new NonEmptyString("Summary"), []);
        var entity2 = Entity.Create(NameType.FromString("Entity2"), new NonEmptyString("Summary"), []);
        var enterprise = new Enterprise("Company", service, new NonEmptyString("Summary"))
        {
            Services = new List<Service>
            {
                new Service(service, new NonEmptyString("Summary"))
                {
                    Entities = new List<NameType> { entity1.Name },
                    References = new List<ReferenceEntity> { new(entity2.Name) }
                }
            },
            Entities = new List<EntityType> { entity1, entity2 }
        };

        var result = enterprise.GetEntitiesByService(service, includeReferences: true).ToList();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be(entity1.Name);
        result[1].Name.Should().Be(entity2.Name);
    }

    [Fact]
    public void GetEntitiesByServiceUsingPredictae_ShouldReturnEntitiesForServiceWithReferences()
    {
        var service = NameType.FromString("Service1");
        var entity1 = Entity.Create(NameType.FromString("Entity1"), new NonEmptyString("Summary 1"), []);
        var entity2 = Entity.Create(NameType.FromString("Entity2"), new NonEmptyString("Summary 2"), []);
        var enterprise = new Enterprise("Company", service, new NonEmptyString("Summary"))
        {
            Services = new List<Service>
            {
                new Service(service, new NonEmptyString("Summary"))
                {
                    Entities = new List<NameType> { entity1.Name },
                    References = new List<ReferenceEntity> { new(entity2.Name) }
                }
            },
            Entities = new List<EntityType> { entity1, entity2 }
        };

        var result = enterprise
            .GetEntitiesByService(service, e => e.Summary.Value.EndsWith('1'), includeReferences: true).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be(entity1.Name);
    }
}

public class EnterpriseTests
{
    [Fact]
    public void ShouldInitializeWithCorrectValues()
    {
        var enterprise = new Enterprise("Company", NameType.FromString("Project"), new NonEmptyString("Summary"))
        {
            Version = new("1.0")
        };

        enterprise.Company.Should().Be("Company");
        enterprise.Project.Should().Be(NameType.FromString("Project"));
        enterprise.Summary.Should().Be(new NonEmptyString("Summary"));
        enterprise.Version.Should().Be(new FileVersion("1.0"));
    }

    [Fact]
    public void ShouldSetServicesCorrectly()
    {
        var services = new List<Service> { new Service(NameType.FromString("TestService"), new("Test summary")) };
        var enterprise = new Enterprise("Company", NameType.FromString("Project"), new("Summary"))
        {
            Services = services
        };

        enterprise.Services.Should().BeEquivalentTo(services);
    }

    [Fact]
    public void ShouldSetEntitiesCorrectly()
    {
        var entities = new List<EntityType>
            { Entity.Create(NameType.FromString("Test Entity"), new("Test summary"), []) };
        var enterprise = new Enterprise("Company", NameType.FromString("Project"), new NonEmptyString("Summary"))
        {
            Entities = entities
        };

        enterprise.Entities.Should().BeEquivalentTo(entities);
    }

    [Fact]
    public void ShouldSetEndpointsCorrectly()
    {
        var endpoints = new List<Endpoint>
        {
            new Endpoint(
                BehaviourVerb.Get,
                NameType.FromString("Endpoint1"),
                new Request(NameType.FromString("Get"), new("Test summary")),
                new Response(NameType.FromString("Response"), new("Test summary")),
                new("/"),
                new("Test summary"))
            {
                Owner = NameType.FromString("Entity"),
            }
        };
        var enterprise = new Enterprise("Company", NameType.FromString("Project"), new NonEmptyString("Summary"))
        {
            Endpoints = endpoints
        };

        enterprise.Endpoints.Should().BeEquivalentTo(endpoints);
    }

    [Fact]
    public void ShouldSetEnumerationsCorrectly()
    {
        var enumerations = new List<Enumeration>
        {
            new Enumeration(NameType.FromString("Enum"), new("Test summary"))
            {
                Values = [new EnumerationValue(1, NameType.FromString("Value1"), new("Test summary"))]
            }
        };
        var enterprise = new Enterprise("Company", NameType.FromString("Project"), new NonEmptyString("Summary"))
        {
            Enumerations = enumerations
        };

        enterprise.Enumerations.Should().BeEquivalentTo(enumerations);
    }
}