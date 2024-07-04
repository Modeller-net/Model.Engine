namespace Domain;

public record ReferenceEntity(NameType Name)
{
    public IEnumerable<NameType> Fields { get; init; } = [];
}