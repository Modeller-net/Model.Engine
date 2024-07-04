namespace Domain;

public abstract record DataTypeType(string Name)
{
    public CollectionTypes CollectionType { get; init; } = CollectionTypes.None;

    public override string ToString() => Name;
}

internal sealed record BoolDataType() : DataTypeType("bool");

internal sealed record ByteDataType() : DataTypeType("byte");

internal sealed record DateDataType(bool Temporal = false) : DataTypeType("DateOnly");

internal sealed record DateTimeDataType(bool Temporal = false) : DataTypeType("DateTime");

internal sealed record DateTimeOffsetDataType(bool Temporal = false) : DataTypeType("DateTimeOffset");

internal sealed record DecimalDataType(int? Precision = null, int? Scale = null) : DataTypeType("decimal");

internal sealed record EntityDataType : DataTypeType
{
    internal EntityDataType(string entityTypeName) : base(entityTypeName)
    {
        EntityTypeName = entityTypeName;
    }
    public string EntityTypeName { get; init; }
}

internal sealed record EnumDataType : DataTypeType
{
    internal EnumDataType(string enumTypeName) : base(enumTypeName)
    {
        EnumTypeName = enumTypeName;
    }
    public string EnumTypeName { get; init; }
}

internal sealed record Int16DataType() : DataTypeType("short");

internal sealed record Int32DataType() : DataTypeType("int");

internal sealed record Int64DataType() : DataTypeType("long");

internal sealed record StringDataType(MinMaxType? MinMax = null, bool Unique = false, bool SupportUnicode = true) : DataTypeType("string");

internal sealed record TimeDataType() : DataTypeType("TimeOnly");

internal sealed record UniqueIdentifierDataType() : DataTypeType("Guid");

internal sealed record LatLongType() : DataTypeType("DbGeography");

internal sealed record ObjectDataType() : DataTypeType("object");

public static class DataType
{
    public static DataTypeType Enum(NonEmptyString enumTypeName) => new EnumDataType(enumTypeName);

    public static DataTypeType Bool() => new BoolDataType();

    public static DataTypeType String(MinMaxType? minMax = null, bool unique = false, bool supportUnicode = true) => new StringDataType(minMax, unique, supportUnicode);

    public static DataTypeType DateTime() => new DateTimeDataType();

    public static DataTypeType Date() => new DateDataType();

    public static DataTypeType Time() => new TimeDataType();

    public static DataTypeType DateTimeOffset() => new DateTimeOffsetDataType();

    public static DataTypeType Byte() => new ByteDataType();

    public static DataTypeType Int16() => new Int16DataType();

    public static DataTypeType Int32() => new Int32DataType();

    public static DataTypeType Int64() => new Int64DataType();

    public static DataTypeType Decimal(int? precision = null, int? scale = null) => new DecimalDataType(precision, scale);

    public static DataTypeType UniqueIdentifier() => new UniqueIdentifierDataType();

    public static DataTypeType Object() => new ObjectDataType();

    public static DataTypeType LatLong() => new LatLongType();

    public static DataTypeType Entity(NonEmptyString entityTypeName) => new EntityDataType(entityTypeName);
}