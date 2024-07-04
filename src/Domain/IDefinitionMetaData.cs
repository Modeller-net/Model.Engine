namespace Domain;

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