namespace Model.Contracts;

/// <summary>
/// Metadata for a definition.
/// </summary>
public interface IDefinitionMetaData
{
    /// <summary>
    /// The version of the generator.
    /// </summary>
    FileVersion Version { get; }

    /// <summary>
    /// The name of the generator.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of what the generator creates.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The type of the entry point for the file.  This is normally used by the calling application to determine where to start.
    /// </summary>
    Type EntryPoint { get; }
}

/// <summary>
/// Metadata for a generator.
/// </summary>
public interface IMetadata
{
    /// <summary>
    /// The version of the generator.
    /// </summary>
    FileVersion Version { get; }

    /// <summary>
    /// The name of the generator.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of what the generator creates.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The type of the entry point for the file.  This is normally used by the calling application to determine where to start.
    /// </summary>
    Type EntryPoint { get; }

    /// <summary>
    /// Other files that this depends on.
    /// </summary>
    IEnumerable<Type> ChildItems { get; }
}

public record LoaderItem(string AbbreviatedFileName, string Name, string Description, string Version);

/// <summary>
/// Represents an item that can be loaded from a file.
/// </summary>
/// <typeparam name="T">The type that will be loaded</typeparam>
public interface ILoaderAsync<T>
{
    /// <summary>
    /// Loads the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>An instance of T</returns>
    Task<ImmutableList<LoaderItem>> LoadAsync(string filePath);

    /// <summary>
    /// Loads the specified file path. This method will return false if the file does not exist.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>True if loaded, otherwise false</returns>
    Task<(bool success, IEnumerable<LoaderItem> instances)> TryLoadAsync(string? filePath);
}
