using Names;

namespace Modeller.Parsers.Models;

public record AttributeType(string Name, Maybe<string> Value);

public record VersionedName(NameType Name, Maybe<string> Version);

public record DataTypeDetail(string DataType, IEnumerable<AttributeType> Attributes);

public record FieldDetail(NameType Name, DataTypeDetail DataType, NonEmptyString Summary, bool IsTemporal);

public record EnumDetail(NameType Name, int Value, NonEmptyString Summary);

public record FlagDetail(NameType Name, int Value, NonEmptyString Summary);

public record OwnerKeyType(string Type, NameType? Name = null);

public abstract record Builder;

public record EnumBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<EnumDetail> Enums) : Builder;

public record FlagBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FlagDetail> Enums) : Builder;

public record RpcBuilder(VersionedName Name, NonEmptyString Summary, string? request, string? response, int timeout) : Builder;

public record EntityBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record TypeBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record DomainBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record ServiceBuilder(VersionedName Name, NonEmptyString Summary, ServiceContent Content) : Builder;

public record ServiceContent(ServiceEnums Enums, ServiceEntities Entities, ServiceReferences References, ServiceCallsRpcs CallsRpcs, ServiceImplementsRpcs ImplementsRpcs);

public record ServiceEnums(IEnumerable<NameType> Enums);

public record ServiceEntities(IEnumerable<NameType> Entities);

public record ServiceReferences(IEnumerable<NameType> References);

public record ServiceCallsRpcs(IEnumerable<NameType> CallsRpcs);

public record ServiceImplementsRpcs(IEnumerable<NameType> ImplementsRpcs);

public record CommandDetail(NameType Name);

public record QueryParamDetail(NameType Name, DataTypeDetail DataType, NonEmptyString Summary);

public record ResponseDetail(IEnumerable<FieldDetail> Fields);

public record EndpointBodyDetail(string Operation, string Path);

public record OwnerDetail(NameType Name);

public record EndpointBuilder(VersionedName Name, OwnerDetail Owner, NonEmptyString Summary) : Builder;

public record EntityKeyBuilder(VersionedName Name, Maybe<OwnerKeyType> Key, bool IsTenant, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;
