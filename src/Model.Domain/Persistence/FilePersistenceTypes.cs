namespace Model.Domain;

public static class FilePersistenceTypes
{
    public static IServiceCollection AddFilePersistenceTypes(this IServiceCollection services)
    {
        List<Assembly> assemblies =
        [
            Assembly.GetAssembly(typeof(JsonPersistenceType))!,
            Assembly.GetExecutingAssembly()
        ];

        foreach (var typeInfo in assemblies.SelectMany(assembly => assembly.GetTypes())
                     .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                         ImplementsGenericInterface(t, typeof(IFilePersistenceType<>))))
        {
            // Register the type for each specific closed generic interface it implements
            foreach (var genericInterface in typeInfo.GetInterfaces()
                         .Where(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(IFilePersistenceType<>)))
            {
                services.AddSingleton(genericInterface, typeInfo);
            }
        }

        return services;
    }

    private static bool ImplementsGenericInterface(Type type, Type genericInterface)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
    }
}