namespace Model.Domain;

public interface IFilePersistenceType<T>
{
    string Value { get; }
    string ToFileExtension();
    string Serialize(T instance);
    T Deserialize(string content);
}