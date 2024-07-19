using System.Collections.Immutable;

namespace Domain;

public record EntityKey(NameType Name, NameType? Owner, ImmutableList<Field> PrimaryKeyFieldList);
