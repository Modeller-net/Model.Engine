namespace Domain;

public record Enumeration(NameType Name, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public required IEnumerable<EnumerationValue> Values { get; init; } = [];
    public bool Flag { get; init; }
}