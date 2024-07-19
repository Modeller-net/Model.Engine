using System.Diagnostics;
using Domain;
using Names;

namespace Modeller.Parsers.Models;

public static class BuilderExtensions
{
    public static bool WhereAllDependenciesMet(this Builder builder, List<Builder> processedBuilders)
    {
        return builder switch
        {
            EntityBuilder => processedBuilders.Any(b=>b is ProjectBuilder),
            EntityKeyBuilder ek => HasEntityBeenBuilt(processedBuilders.Where(pb=>pb is EntityBuilder).Cast<EntityBuilder>(), ek.Name.Value),
            _ => true
        };

        bool HasEntityBeenBuilt(IEnumerable<EntityBuilder> ebs, NameType name)
        {
            return ebs.Any(e=>e.Name.Value.Value == name);
        }
    }

    public static Enterprise Process(this Builder builder, Enterprise enterprise)
    {
        if (builder is EntityBuilder eb)
        {
            var fields = eb.Fields.Select(f => new Field(f.Name, f.DataType.Create(), f.Summary));
            var e = Entity.Create(eb.Name.Value, eb.Summary, fields);

            var current = enterprise.Entities.FirstOrDefault(e => e.Name.Value == eb.Name.Value);
            if (current is null)
                enterprise.AddEntity(e);
            else
                enterprise.ReplaceEntity(current,e);
        }
        else if (builder is EntityKeyBuilder ek)
        {
            var entity = enterprise.Entities.FirstOrDefault(ce => ce.Name.Value == ek.Name.Value);
            Debug.Assert(entity != null);

            var owner = ek.Key.HasValue ? ek.Key.Value : null;
            var fields = ek.Fields.Select(f => new Field(f.Name, f.DataType.Create(), f.Summary));
            
            var e = new EntityKey(ek.Name.Value, owner?.Name, fields.ToImmutableList());
            
            var newEntity = entity with { Key = e};
            enterprise.ReplaceEntity(entity,newEntity);
        }
        return enterprise;
    }

    public static Enterprise Process(this ProjectBuilder builder)
    {
        var company = builder.Attributes["Company"];
        return new Enterprise(company, builder.Name.Value, builder.Summary);
    }
}