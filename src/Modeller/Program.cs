using Modeller.NET.Tool.Core;

var services = new ServiceCollection()
    .AddLogging(configure =>
        configure.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
            .Enrich.With<LoggingEnricher>()
            .WriteTo.Map(LoggingEnricher.LogFilePathPropertyName,
                (logFilePath, wt) => wt.File($"{logFilePath}"), 1)
            .CreateLogger()
        ));
        
services.AddSingleton(_ =>
{
    var templateFolder = Settings.DefaultTemplateFolder.FullName;
    var definitionFolder = Settings.DefaultDefinitionFolder.FullName;
    var outputFolder = Settings.DefaultOutputFolder.FullName;
    return new Settings(templateFolder, definitionFolder, outputFolder, Targets.Default);
});
services.AddSingleton<ISettingsService, SettingsService>();
services.AddSingleton<ILoader<Settings>, JsonSettingsLoader>();
services.AddSingleton<IDefinitions, Definitions>();
services.AddSingleton<FileSystemMonitor>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.SetApplicationName("entity");
    config.ValidateExamples();
    config.SetInterceptor(new LogInterceptor()); // add the interceptor
    // config.AddBranch("list",list=>
    // {
    //     list.SetDescription("List the available definitions or templates");
    //     list.AddCommand<ListDefinitionsCommand>("definitions")
    //         .WithExample("list", "definitions", "--folder", Modeller.Generator.Outputs.Path.Combine("Modeller","src","modeller.definitions"));
    //     list.AddCommand<ListTemplatesCommand>("templates")
    //         .WithExample("list", "templates", "--folder", Modeller.Generator.Outputs.Path.Combine("Modeller","src","modeller.templates"), "--target", "net8.0");
    // });
    // config.AddCommand<ConvertCommand>("convert")
    //     .WithDescription("Convert a folder structure to a template project")
    //     .WithExample("convert", Modeller.Generator.Outputs.Path.Combine("MyProject"), Modeller.Generator.Outputs.Path.Combine("c:","temp","myProjectTemplate"), "MyProject");
    // config.AddCommand<BuildCommand>("build")
    //     .WithDescription("Build a definition using a specific template")
    //     .WithExample("build", "ApiSolution", "CaseDomain", "--definitions", "../src/Modeller.Definitions", "--templates", "../src/Modeller.Templates", "--output", "c:/playschool");
    config.AddCommand<SettingsCommand>("settings")
        .WithDescription("Manage the settings for the tool")
        .WithExample("settings","--filepath", "c:/playschool/Settings.json");
    config.AddCommand<ValidateCommand>("validate")
        .WithDescription("Validate a definition")
        .WithExample("validate", "CaseDomain", "--definitions", "../src/Modeller.Definitions" );
    config.AddCommand<DefinitionCommand>("definition")
        .WithDescription("Write the definition to a directory")
        .WithExample("definition", "NewBranch", "--definitions", "../src/Modeller.Definitions");
    config.AddCommand<WatchCommand>("watch")
        .WithDescription("Watch for definition changes and generate the output automatically")
        .WithExample("watch", "--definitions", "../src/Modeller.Definitions", "--templates", "../src/Modeller.Templates", "--output", "c:/playschool");
});

return app.Run(args);
