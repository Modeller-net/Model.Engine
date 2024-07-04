namespace Modeller.NET.Tool.Infrastructure;

internal class LogInterceptor : ICommandInterceptor
{
    public static readonly LoggingLevelSwitch LogLevel = new();

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is not LogCommandSettings logSettings) return;
        
        LoggingEnricher.Path = logSettings.LogFile ?? "modeller.log";
        LogLevel.MinimumLevel = logSettings.LogLevel;
    }
}