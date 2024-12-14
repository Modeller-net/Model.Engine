namespace Model.Domain;

public record XmlPersistenceType : FilePersistenceType<Settings>
{
    public XmlPersistenceType() : base("Xml") { }

    public override string Serialize(Settings settings)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, settings);
        return stringWriter.ToString();
    }

    public override Settings Deserialize(string content)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
        using var stringReader = new StringReader(content);
        return (Settings?)serializer.Deserialize(stringReader) ?? Settings.Default;
    }
}