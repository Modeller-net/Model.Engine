namespace Domain;

public record Enterprise(string Company, NameType Project, NonEmptyString Summary) : DocumentationBase(Summary)
{
    private readonly List<Enumeration> _enumerations = [];
    private readonly List<Service> _services = [];
    private readonly List<Endpoint> _endpoints = [];
    private readonly List<EntityType> _entities = [];

    public FileVersion Version { get; init; } = FileVersion.Initial;

    public IEnumerable<Service> Services
    {
        get => _services;
        init => _services = value.ToList();
    }

    public IEnumerable<EntityType> Entities
    {
        get => _entities;
        init => _entities = value.ToList();
    }

    public IEnumerable<Endpoint> Endpoints
    {
        get => _endpoints;
        init => _endpoints = value.ToList();
    }

    public IEnumerable<Enumeration> Enumerations
    {
        get => _enumerations;
        init => _enumerations = value.ToList();
    }

    public void ReplaceEntity(EntityType old, EntityType @new)
    {
        _entities.Remove(old);
        _entities.Add(@new);
    }
    
    public void AddEntity(EntityType entity)
    {
        
        _entities.Add(entity);
    }
}