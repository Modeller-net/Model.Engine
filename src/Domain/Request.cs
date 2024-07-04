using System.Collections.Immutable;

namespace Domain;

public record Request(NameType Name, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public IEnumerable<Field> QueryParameters { get; init; } = [];
    public IDictionary<string, string> Headers { get; init; } = ImmutableDictionary<string, string>.Empty;
    public string? MediaType { get; init; } = "application/json";
    public RequestBody? Body { get; init; }
}