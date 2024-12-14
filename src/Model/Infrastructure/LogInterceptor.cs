using Model.Commands;

namespace Model.Infrastructure;

internal sealed class LogInterceptor : ICommandInterceptor
{
    public static readonly LoggingLevelSwitch LogLevel = new();

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is not LogCommandSettings logSettings) return;

        LoggingEnricher.Path = logSettings.LogFile ?? "modeller.log";
        LogLevel.MinimumLevel = logSettings.LogLevel;
    }
}