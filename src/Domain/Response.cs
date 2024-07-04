namespace Domain;

public record Response(NameType Name, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public IEnumerable<Field> Fields { get; init; } = [];
    public string? MediaType { get; init; } = "application/json";
}