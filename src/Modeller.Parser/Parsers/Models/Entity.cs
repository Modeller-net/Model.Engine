using Names;

namespace Modeller.Parsers.Models;

public record AttributeType(string Name, Maybe<string> Value);

public record DataTypeDetail(string DataType, IEnumerable<AttributeType> Attributes);

public record OwnerKeyType(string Type, NameType? Name = null);

public record EntityBuilder(VersionedEntityName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields);

public record EntityKeyBuilder(VersionedEntityName Name, Maybe<OwnerKeyType> Key, bool IsTenant, NonEmptyString Summary, IEnumerable<FieldDetail> Fields);

public record FieldDetail(NameType Name, DataTypeDetail DataType, NonEmptyString Summary);

public record VersionedEntityName(NameType Name, Maybe<string> Version);