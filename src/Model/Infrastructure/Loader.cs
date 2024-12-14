using Model.Domain;

namespace Model.Infrastructure;

public sealed class AssemblyLoader
{
    public static Assembly Load(string filePath)
    {
        using var loader = PluginLoader.CreateFromAssemblyFile(filePath, sharedTypes: [typeof(Settings)]);
        return loader.LoadDefaultAssembly();
    }
}

public class Loader<T>(ILogger<Loader<T>> logger) : ILoaderAsync<T>
{
    private readonly ILogger<Loader<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    async Task<ImmutableList<LoaderItem>> ILoaderAsync<T>.LoadAsync(string filePath) =>
        await ProcessAsync(filePath);

    async Task<(bool success, IEnumerable<LoaderItem> instances)> ILoaderAsync<T>.TryLoadAsync(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return (false, []);
        }

        try
        {
            var instances = await ProcessAsync(filePath);
            return (instances.Count != 0, instances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load");
        }

        return (false, []);
    }

    private async Task<ImmutableList<LoaderItem>> ProcessAsync(string filePath)
    {
        _logger.LogDebug("Processing {Folder}", filePath);

        var local = new DirectoryInfo(filePath);
        var list = new List<LoaderItem>();
        if (!local.Exists)
        {
            _logger.LogDebug("{Folder} does not exist", filePath);
            return [];
        }

        var builder = ImmutableList.CreateBuilder<LoaderItem>();
        await foreach (var item in AddFilesAsync(list, local))
        {
            _logger.LogDebug(" - {ItemName} ({Version})", item.Name, item.Version);
            builder.Add(item);
        }

        _logger.LogDebug("Finished Processing {Folder}. {ListCount} definition(s) found", filePath, list.Count);
        return builder.ToImmutable();
    }

    private async IAsyncEnumerable<LoaderItem> AddFilesAsync(List<LoaderItem> list, DirectoryInfo folder)
    {
        _logger.LogDebug("Adding files {Folder}", folder.FullName);

        foreach (var subFolder in folder.GetDirectories())
        {
            await foreach (var item in AddFilesAsync(list, subFolder))
            {
                yield return item;
            }
        }

        foreach (var file in folder.GetFiles("*.dll"))
        {
            var deps = file.FullName[..^3] + "deps.json";
            if (!System.IO.File.Exists(deps))
            {
                continue;
            }

            foreach (var item in LoadDefinitions(list, file))
            {
                yield return item;
            }
        }
    }

    private IEnumerable<LoaderItem> LoadDefinitions(List<LoaderItem> list, FileInfo file)
    {
        IEnumerable<TypeInfo> dt =[];
        try
        {
            var ass = AssemblyLoader.Load(file.FullName);
            dt = ass.DefinedTypes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get {Name} defined types", file.FullName);
            yield break;
        }

        var metaDataTypes = dt.Where(t => t.ImplementedInterfaces.Any(it => it.Name == typeof(T).Name));
        foreach (var type in metaDataTypes)
        {
            if (type.IsAbstract || type.IsInterface || !type.IsPublic)
            {
                continue;
            }

            var obj = Activator.CreateInstance(type);
            var result = obj switch
            {
                null => null,
                IMetadata generator => new LoaderItem(
                    FileHelper.GetAbbreviatedFilename(file.FullName).filename,
                    generator.Name,
                    generator.Description,
                    generator.Version.ToString()),
                IDefinitionMetaData definition => new LoaderItem(
                    FileHelper.GetAbbreviatedFilename(file.FullName).filename,
                    definition.Name,
                    definition.Description,
                    definition.Version.ToString()),
                _ => CreateDetail(type, obj) is { } md
                    ? new LoaderItem(
                        FileHelper.GetAbbreviatedFilename(file.FullName).filename,
                        md.Name,
                        md.Description,
                        md.Version.ToString())
                    : null
            };

            if (result == null) continue;

            list.Add(result);
            yield return result;
        }
    }

    private static TempDefinitionDetail? CreateDetail(TypeInfo type, object? obj)
    {
        var name = type.GetProperty("Name")?.GetValue(obj)?.ToString();
        if (string.IsNullOrEmpty(name) || type.GetProperty("EntryPoint")?.GetValue(obj) is not Type entryPoint)
        {
            return null;
        }

        var description = type.GetProperty("Description")?.GetValue(obj)?.ToString() ?? string.Empty;
        var version = type.GetProperty("Version")?.GetValue(obj)?.ToString();
        var v = FileVersion.Parse(version);
        return new(name, description, entryPoint, v);
    }

    private class TempDefinitionDetail : IDefinitionMetaData
    {
        internal TempDefinitionDetail(string name, string description, Type entryPoint, FileVersion version)
        {
            Name = name;
            Description = description;
            EntryPoint = entryPoint;
            Version = version;
        }

        public string Name { get; }
        public string Description { get; }
        public Type EntryPoint { get; }
        public FileVersion Version { get; }
    }
}
public static class FileHelper
{
    public static (string filename, FileVersion version) GetAbbreviatedFilename(string filePath)
    {
        var filename = Path.GetFileNameWithoutExtension(filePath);
        if(string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename cannot be empty", nameof(filePath));
        
        var parts = filename.Split('.');
        if (parts.Length == 0)
        {
            return (filename, new());
        }

        var f = string.Empty;
        var v = string.Empty;
        foreach (var t in parts)
        {
            if (t.StartsWith("v", StringComparison.InvariantCultureIgnoreCase) &&
                int.TryParse(t.AsSpan(1), out var number1))
            {
                v += number1 + ".";
            }
            else if (int.TryParse(t, out var number2))
            {
                v += number2 + ".";
            }
            else
            {
                f += t + ".";
            }
        }

        var fn = string.IsNullOrEmpty(f) ? string.Empty : f[..^1];
        var ve = string.IsNullOrEmpty(v)
            ? new()
            : new FileVersion(v[..^1]);
        return (fn, ve);
    }

    public static bool UpdateLocalGenerators(string? serverFolder = null, string? localFolder = null,
        bool overwrite = false, Action<string>? output = null)
    {
        if (string.IsNullOrWhiteSpace(serverFolder) || string.IsNullOrWhiteSpace(localFolder))
        {
            return false;
        }

        var server = new DirectoryInfo(serverFolder);
        var local = new DirectoryInfo(localFolder);

        if (!server.Exists)
        {
            return false;
        }

        DirectoryCopy(server, local, true, overwrite, output);
        return true;
    }

    private static void DirectoryCopy(DirectoryInfo sourceDirName, DirectoryInfo destDirName, bool copySubDirs,
        bool overwrite, Action<string>? output)
    {
        if (!sourceDirName.Exists)
        {
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " +
                                                           sourceDirName);
        }

        var dirs = sourceDirName.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!destDirName.Exists)
        {
            output?.Invoke($"creating {destDirName.FullName}");
            destDirName.Create();
        }

        // Get the files in the directory and copy them to the new location.
        var files = sourceDirName.GetFiles();
        foreach (var file in files)
        {
            var tempPath = Path.Combine(destDirName.FullName, file.Name);
            if (File.Exists(tempPath) && !overwrite)
            {
                continue;
            }

            output?.Invoke($"copying {file.Name} to {destDirName.Name}");
            file.CopyTo(tempPath, false);
        }

        if (!copySubDirs) return;

        foreach (var subDir in dirs)
        {
            var tempPath = new DirectoryInfo(Path.Combine(destDirName.FullName, subDir.Name));
            DirectoryCopy(subDir, tempPath, copySubDirs, overwrite, output);
        }
    }
}
