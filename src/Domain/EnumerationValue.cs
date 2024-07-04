namespace Domain;

public record EnumerationValue(byte Value, NameType Name, NonEmptyString Summary) : DocumentationBase(Summary);