namespace Model.Commands;

public class LogCommandSettings : CommandSettings
{
    [CommandOption("--logFile")]
    [Description("Path and file name for logging")]
    public required string LogFile { get; set; }

    [CommandOption("--logLevel")]
    [Description("Minimum level for logging")]
    [TypeConverter(typeof(VerbosityConverter))]
    [DefaultValue(LogEventLevel.Information)]
    public LogEventLevel LogLevel { get; set; }
}

public class ModelSettingsCommandSettings : LogCommandSettings
{
    [CommandOption("--name")]
    [Description("The name of the settings file.")]
    [DefaultValue("settings")]
    public string? Name { get; init; }

    [CommandOption("--type")]
    [Description("The persistence type (json/yaml/xml).")]
    [DefaultValue("Yaml")]
    public required string Type { get; init; } = "Yaml";
}

internal sealed class VerbosityConverter : TypeConverter
{
    private readonly Dictionary<string, LogEventLevel> _lookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "d", LogEventLevel.Debug },
        { "v", LogEventLevel.Verbose },
        { "i", LogEventLevel.Information },
        { "w", LogEventLevel.Warning },
        { "e", LogEventLevel.Error },
        { "f", LogEventLevel.Fatal },
        { "debug", LogEventLevel.Debug },
        { "verbose", LogEventLevel.Verbose },
        { "information", LogEventLevel.Information },
        { "warning", LogEventLevel.Warning },
        { "error", LogEventLevel.Error },
        { "fatal", LogEventLevel.Fatal }
    };

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue)
        {
            throw new NotSupportedException("Can't convert value to verbosity.");
        }

        var result = _lookup.TryGetValue(stringValue, out var verbosity);
        if (result)
        {
            return verbosity;
        }

        const string format = "The value '{0}' is not a valid verbosity.";
        var message = string.Format(CultureInfo.InvariantCulture, format, value);
        throw new InvalidOperationException(message);
    }
}
