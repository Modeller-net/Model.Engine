using Model.Infrastructure;

namespace Model;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddModelDependencies(this IServiceCollection services)
    {
        //services
            // .AddSingleton<ILoaderAsync<IDefinitionMetaData>, Loader<IDefinitionMetaData>>()
            // .AddSingleton<ILoaderAsync<IMetadata>, Loader<IMetadata>>()
            // .AddSingleton<IBuilder, Builder>()
            // .AddSingleton<IContext, Context>()
            // .AddSingleton<ILoader<IDefinitionItem>, DefinitionLoader>()
            // .AddSingleton<ILoader<IEnumerable<IGeneratorItem>>, GeneratorLoader>()
            // .AddSingleton<ICodeGenerator, CodeGenerator>()
            // .AddSingleton<IOutputStrategy, OutputStrategy>()
            // .AddSingleton<IFileCreator, CreateFile>()
            // .AddSingleton<IFileCreator, CreateSnippet>()
            // .AddSingleton<IFileCreator, CreateProject>()
            // .AddSingleton<IFileCreator, CreateSolution>()
            // .AddSingleton<IFileCreator, FileCopier>()
            // .AddSingleton<IFileCreator, FolderCopier>()
            // .AddSingleton<IFileCreator, CreateFileGroup>()
            // .AddSingleton<IFileWriter, FileWriter>();

        return services.AddSerilogLogging();
    }

    internal static IServiceCollection AddSerilogLogging(this IServiceCollection services)
    {
        return services.AddLogging(configure =>
            configure.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
                .Enrich.With<LoggingEnricher>()
                .WriteTo.Map(LoggingEnricher.LogFilePathPropertyName,
                    (logFilePath, wt) => wt.File($"{logFilePath}"), 1)
                .CreateLogger()
            ));
    }
}
