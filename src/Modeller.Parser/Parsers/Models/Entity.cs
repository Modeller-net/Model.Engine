using Domain;
using Names;

namespace Modeller.Parsers.Models;

public record AttributeType(string Name, Maybe<string> Value);

public record VersionedName(NameType Value, Maybe<string> Version);

public record DataTypeDetail(string DataType, IEnumerable<AttributeType> Attributes)
{
    public DataTypeType Create()
    {
        return DataType switch
        {
            "boolean" => Domain.DataType.Bool(),
            "string" => CreateStringType(),
            "datetime" => Domain.DataType.DateTime(),
            "date" => Domain.DataType.Date(),
            "time" => Domain.DataType.Time(),
            "datetimeoffset" => Domain.DataType.DateTimeOffset(),
            "byte" => Domain.DataType.Byte(),
            "currency" => Domain.DataType.Currency(),
            "integer" => Domain.DataType.Int32(),
            "long" => Domain.DataType.Int64(),
            "double" => Domain.DataType.Decimal(18,2),
            "decimal" => Domain.DataType.Decimal(),
            "Guid" => Domain.DataType.UniqueIdentifier(),
            //"object" => Domain.DataType.Entity(),
            _ => Domain.DataType.String()
        };
    }
    
    private DataTypeType CreateStringType()
    {
        int? min = int.TryParse(Attributes.FirstOrDefault(a => a.Name == "min")?.Value.GetValueOrDefault(), out var 
            mi) ? mi : null;
        int? max = int.TryParse(Attributes.FirstOrDefault(a => a.Name == "max")?.Value.GetValueOrDefault(), out var ma)
            ? ma : null;
        var minMax = MinMax.Set(min, max);
        //var required = Attributes.All(a => a.Name != "optional");
        return Domain.DataType.String(minMax);
    }
}

public static class DataTypeDetailExtensions
{
    public static DataTypeType From(this DataTypeDetail dataType)
    {
        return dataType.Create();
    }
}

public record FieldDetail(NameType Name, DataTypeDetail DataType, NonEmptyString Summary, bool IsTemporal);

public record EnumDetail(NameType Name, int Value, NonEmptyString Summary);

public record FlagDetail(NameType Name, int Value, NonEmptyString Summary);

public record OwnerKeyType(string Type, NameType? Name = null);

public abstract record Builder;

public record EnumBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<EnumDetail> Enums) : Builder;

public record FlagBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FlagDetail> Enums) : Builder;

public record RpcBuilder(VersionedName Name, NonEmptyString Summary, string? request, string? response, int timeout) : Builder;

public record ProjectBuilder(VersionedName Name, NonEmptyString Summary, Dictionary<string, string> Attributes) : 
    Builder;

public record EntityBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record TypeBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record DomainBuilder(VersionedName Name, NonEmptyString Summary, IEnumerable<FieldDetail> Fields) : Builder;

public record EndpointBuilder(VersionedName Name, OwnerDetail Owner, NonEmptyString Summary) : Builder;

public record EntityKeyBuilder(
    VersionedName Name,
    Maybe<OwnerKeyType> Key,
    bool IsTenant,
    NonEmptyString Summary,
    IEnumerable<FieldDetail> Fields) : Builder;

public record ServiceBuilder(VersionedName Name, NonEmptyString Summary, ServiceContent Content) : Builder;

public record ServiceContent(ServiceEnums Enums, ServiceEntities Entities, ServiceReferences References, ServiceCallsRpcs CallsRpcs, ServiceImplementsRpcs ImplementsRpcs);

public record ServiceEnums(IEnumerable<NameType> Items);

public record ServiceEntities(IEnumerable<NameType> Items);

public record ServiceReferences(IEnumerable<ReferenceNames> Items);

public record ServiceCallsRpcs(IEnumerable<NameType> Items);

public record ServiceImplementsRpcs(IEnumerable<NameType> Items);

public record ReferenceNames(NameType Name, IEnumerable<NameType> References);

public record CommandDetail(NameType Name);

public record QueryParamDetail(NameType Name, DataTypeDetail DataType, NonEmptyString Summary);

public record ResponseDetail(IEnumerable<FieldDetail> Fields);

public record EndpointBodyDetail(string Operation, string Path);

public record OwnerDetail(NameType Name);