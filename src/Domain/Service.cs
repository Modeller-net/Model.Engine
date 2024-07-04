namespace Domain;

public record Service(NameType Name, NonEmptyString Summary) : DocumentationBase(Summary)
{
    private static readonly NameType Tenant = NameType.FromString("Tenant");

    public IEnumerable<NameType> Entities { get; init; } = [];
    public IEnumerable<NameType> Enumerations { get; init; } = [];
    public IEnumerable<ReferenceEntity> References { get; init; } = [];
    public IEnumerable<Page> Pages { get; init; } = [];

    public bool HasTenant()
    {
        return Entities.Any(e => e == Tenant) || References.Any(r => r.Name == Tenant);
    }
}