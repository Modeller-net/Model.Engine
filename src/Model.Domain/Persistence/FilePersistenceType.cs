namespace Model.Domain;

public abstract record FilePersistenceType<T>(string Value) : IFilePersistenceType<T>
{
    public virtual string ToFileExtension() => ValidateFileExtension(Value);

    private string ValidateFileExtension(string value)
    {
        var ext = value.ToLowerInvariant().TrimStart('.');
        return $".{ext}";
    }

    /// <summary>
    /// Serializes the given Settings object to a string.
    /// </summary>
    public abstract string Serialize(T instance);

    /// <summary>
    /// Deserializes the given string to a Settings object.
    /// </summary>
    public abstract T Deserialize(string content);
}