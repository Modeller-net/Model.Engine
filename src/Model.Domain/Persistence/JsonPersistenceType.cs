namespace Model.Domain;

public record JsonPersistenceType : FilePersistenceType<Settings>
{
    private readonly ILogger<JsonPersistenceType> _logger;

    public JsonPersistenceType(ILogger<JsonPersistenceType> logger) : base("Json")
    {
        _logger = logger;
    }

    public override string Serialize(Settings settings)
    {
        _logger.LogDebug("Serializing settings to JSON");
        return JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
    }

    public override Settings Deserialize(string content) => JsonSerializer.Deserialize<Settings>(content) ?? Settings.Default;
}