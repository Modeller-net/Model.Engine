namespace Domain;

public record RequestBody()
{
    public string? MediaType { get; init; } = "application/json";
    public IEnumerable<Field> Fields { get; init; } = [];
}