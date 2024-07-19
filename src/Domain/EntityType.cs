namespace Domain;

public record EntityType(NameType Name, NonEmptyString Summary) : DocumentationBase(Summary)
{
    public EntityKey? Key { get; init; }
    public EntityAttributesType Attributes { get; init; } = EntityAttributes.None;
    public NameType? InheritedModel { get; init; }
    public string? Schema { get; init; }
    public required IEnumerable<Field> Fields { get; init; }
    public IEnumerable<Index> Indexes { get; init; } = [];
}

public record EntityAttributesType(long Value)
{
    public bool ShouldCache => (Value & 1) == 1;
    public bool ShouldAudit => (Value & 2) == 2;
    public bool IsRoot => (Value & 4) == 4;
    public bool IsValueObject => (Value & 8) == 8;
    public bool AsReference => (Value & 16) == 16;

    public CrudSupport SupportCrud => (CrudSupport)((Value >> 5) & 7);
}

public static class EntityAttributes
{
    public static EntityAttributesType None => new (0);

    public static EntityAttributesType Cache(this EntityAttributesType ett, bool value = true)
        => new(Value: ett.Value | 1);


    public static EntityAttributesType Audit(this EntityAttributesType ett, bool value = true)
        => new (Value: ett.Value | 2);

    public static EntityAttributesType Root(this EntityAttributesType ett, bool value = true)
        => new (Value: ett.Value | 4);

    public static EntityAttributesType ValueObject(this EntityAttributesType ett, bool value = true)
        => new (Value: ett.Value | 8);

    public static EntityAttributesType Reference(this EntityAttributesType ett, bool value = true)
        => new (Value: ett.Value | 16);

    public static EntityAttributesType SupportCrud(this EntityAttributesType ett, CrudSupport value = CrudSupport.None)
        => new (Value: ett.Value | (long)value);
}

public static class Entity
{
    public static EntityType Create(NameType name, NonEmptyString summary, IEnumerable<Field> fields)
        => new (Name: name, Summary: summary)
        {
            Fields = fields,
        };
}

