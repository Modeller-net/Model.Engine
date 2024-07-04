namespace Domain;

public record Endpoint(BehaviourVerb Verb, NameType Name, Request Request, Response Response, NonEmptyString RouteTemplate, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public BehaviourVerb Verb { get; } = Verb;
    public NameType Name { get; } = Name;
    public Request Request { get; } = Request;
    public Response Response { get; } = Response;
    public NonEmptyString RouteTemplate { get; } = RouteTemplate;
    public required NameType Owner { get; init; }
    public string? Event { get; init; }
}