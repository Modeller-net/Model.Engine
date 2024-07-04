namespace Modeller.NET.Tool.Commands;

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