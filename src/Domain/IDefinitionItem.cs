namespace Domain;

/// <summary>
/// Contract for a valid definition dll.
/// </summary>
public interface IDefinitionItem
{
    /// <summary>
    /// The abbreviated file name of the definition dll.
    /// </summary>
    string AbbreviatedFileName { get; }

    /// <summary>
    /// The full path to the definition dll.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// The GeneratorDetails.Instance for the definition dll.
    /// </summary>
    IDefinitionMetaData Metadata { get; }

    /// <summary>
    /// The type of the definition dll.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Get the instance of the definition dll.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    IDefinition Instance(params object[] args);
}