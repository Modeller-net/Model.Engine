namespace Domain;

public static class EnterpriseExtensions
{
    public static IEnumerable<EntityType> GetEntitiesByService(this Enterprise enterprise, NameType service, Func<EntityType, bool>? predicate = null, bool includeReferences = true)
    {
        var s = enterprise.Services.First(s => s.Name == service);
        var query = enterprise.Entities.Where(e => s.Entities.Contains(e.Name));
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        foreach (var entity in query)
        {
            yield return entity;
        }

        if (!includeReferences)
        {
            yield break;
        }
        foreach (var reference in s.References)
        {
            var refQuery = enterprise.Entities;
            if (predicate is not null)
            {
                refQuery = refQuery.Where(predicate);
            }
            var en = refQuery.FirstOrDefault(e => e.Name == reference.Name);
            if (en is null)
            {
                continue;
            }
            yield return en with
            {
                Attributes = EntityAttributes.None
                    .Reference()
                    .SupportCrud(CrudSupport.Read),
                Schema = "ref",
                Fields = en.Fields.Where(f => reference.Fields.Contains(f.Name)),
                Indexes = []
            };
        }
    }
}
