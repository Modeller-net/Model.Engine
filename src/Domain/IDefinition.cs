namespace Domain;

/// <summary>
/// The contract used to create a definition
/// </summary>
public interface IDefinition
{
    /// <summary>
    /// The command to use to create the generated output
    /// </summary>
    /// <returns>A <seealso cref="Enterprise"/> type</returns>
    Enterprise Create();
}