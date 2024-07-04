using System.IO.Abstractions;

namespace Generator;

public sealed class DefinitionWriter(IFileSystem fileSystem, ILogger<DefinitionWriter> logger, IWriter writer) : IDefinitionWriter
{
    public ILogger<DefinitionWriter> Logger { get; } = logger;

    public void Write(string rootFolder, Enterprise instance)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(rootFolder));
        ArgumentNullException.ThrowIfNull(nameof(instance));

        var di = fileSystem.Directory.CreateDirectory(rootFolder);
        if (!di.Exists) return;

        var file = Path.Combine(di.FullName, $"{instance.Project}.def");
        writer.Write(file, instance);

        var entitiesFolder = di.CreateSubdirectory("entities");
        foreach (var entity in instance.Entities)
        {
            var entityFolder = entitiesFolder.CreateSubdirectory($"{entity.Name}");
            var filePath = Path.Combine(entityFolder.FullName, $"{entity.Name}.entity");
            writer.Write(filePath, entity);
            var keyPath = Path.Combine(entityFolder.FullName, $"{entity.Name}.key");
            writer.Write(keyPath, entity.Key);
        }

        var enumerationFolder = di.CreateSubdirectory("enumerations");
        foreach (var enumeration in instance.Enumerations)
        {
            var filePath = Path.Combine(enumerationFolder.FullName, $"{enumeration.Name}.enum");
            writer.Write(filePath, enumeration);
        }

        var serviceFolder = di.CreateSubdirectory("services");
        foreach (var service in instance.Services)
        {
            var filePath = Path.Combine(serviceFolder.FullName, $"{service.Name}.service");
            writer.Write(filePath, service);
        }

        var endpointFolder = di.CreateSubdirectory("endpoints");
        foreach (var endpoint in instance.Endpoints)
        {
            var filePath = Path.Combine(endpointFolder.FullName, $"{endpoint.Name}.endpoint");
            writer.Write(filePath, endpoint);
        }
    }
}
