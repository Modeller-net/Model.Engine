using System.Diagnostics;
using Domain;
using Names;

namespace Modeller.Parsers.Models;

public static class BuilderExtensions
{
    public static Enterprise Process(this Builder builder, Enterprise enterprise)
    {
        return builder switch
        {
            DomainBuilder eb => ProcessDomain(enterprise, eb),
            EntityBuilder eb => ProcessEntity(enterprise, eb),
            EntityKeyBuilder ek => ProcessEntityKey(enterprise, ek),
            EnumBuilder eb => ProcessEnumeration(enterprise, eb),
            FlagBuilder eb => ProcessFlag(enterprise, eb),
            _ => enterprise
        };
    }

    public static Enterprise Process(this ProjectBuilder builder) => 
        new(builder.Attributes["Company"], builder.Name.Value, builder.Summary);

    public static bool WhereAllDependenciesMet(this Builder builder, List<Builder> processedBuilders)
    {
        return builder switch
        {
            EntityBuilder => processedBuilders.Any(b=>b is ProjectBuilder),
            EntityKeyBuilder ek => HasEntityBeenBuilt(processedBuilders
                .Where(pb=>pb is EntityBuilder)
                .Cast<EntityBuilder>(), ek.Name.Value),
            _ => true
        };

        bool HasEntityBeenBuilt(IEnumerable<EntityBuilder> ebs, NameType name)
        {
            return ebs.Any(e=>e.Name.Value.Value == name);
        }
    }

    private static Enterprise ProcessDomain(Enterprise enterprise, DomainBuilder eb)
    {
        var fields = eb.Fields.Select(f => new Field(f.Name, f.DataType.Create(), f.Summary));
        var entity = Entity.Create(eb.Name.Value, eb.Summary, fields);
        var current = enterprise.Entities.FirstOrDefault(e => e.Name.Value == eb.Name.Value);
        if (current is null)
            enterprise.AddEntity(entity);
        else
            enterprise.ReplaceEntity(current, entity);
        return enterprise;
    }

    private static Enterprise ProcessEntity(Enterprise enterprise, EntityBuilder eb)
    {
        var fields = eb.Fields.Select(f => new Field(f.Name, f.DataType.Create(), f.Summary));
        var entity = Entity.Create(eb.Name.Value, eb.Summary, fields);
        var current = enterprise.Entities.FirstOrDefault(e => e.Name.Value == eb.Name.Value);
        if (current is null)
            enterprise.AddEntity(entity);
        else
            enterprise.ReplaceEntity(current, entity);
        return enterprise;
    }

    private static Enterprise ProcessEntityKey(Enterprise enterprise, EntityKeyBuilder ek)
    {
        var entity = enterprise.Entities.FirstOrDefault(ce => ce.Name.Value == ek.Name.Value);
        Debug.Assert(entity != null);

        var owner = ek.Key.HasValue ? ek.Key.Value : null;
        var fields = ek.Fields.Select(f => new Field(f.Name, f.DataType.Create(), f.Summary));
        var e = new EntityKey(ek.Name.Value, owner?.Name, fields.ToImmutableList());
        var newEntity = entity with { Key = e};
        enterprise.ReplaceEntity(entity,newEntity);
        return enterprise;
    }
    
    private static Enterprise ProcessEnumeration(Enterprise enterprise, EnumBuilder eb)
    {
        var es = eb.Enums.Select(f=>new EnumerationValue(Convert.ToByte(f.Value), f.Name, f.Summary));
        var en = new Enumeration(eb.Name.Value, eb.Summary)
        {
            Values = es.ToImmutableList()
        };
        var current = enterprise.Enumerations.FirstOrDefault(e => e.Name.Value == eb.Name.Value);
        if (current is null)
            enterprise.AddEnumeration(en);
        else
            enterprise.ReplaceEnumeration(current, en);
        return enterprise;
    }
    
    private static Enterprise ProcessFlag(Enterprise enterprise, FlagBuilder eb)
    {
        var es = eb.Enums.Select(f=>new EnumerationValue(Convert.ToByte(f.Value), f.Name, f.Summary));
        var en = new Enumeration(eb.Name.Value, eb.Summary)
        {
            Flag = true,
            Values = es.ToImmutableList()
        };
        var current = enterprise.Enumerations.FirstOrDefault(e => e.Name.Value == eb.Name.Value);
        if (current is null)
            enterprise.AddEnumeration(en);
        else
            enterprise.ReplaceEnumeration(current, en);
        return enterprise;
    }
}