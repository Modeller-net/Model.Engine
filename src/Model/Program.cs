using Model;
using Model.Commands;
using Model.Domain;
using Model.Infrastructure;

var services = new ServiceCollection()
    .AddModelDependencies()
    .AddFilePersistenceTypes();

//.AddSingleton<ILoader<IDefinitionMetaData>, Loader<IDefinitionMetaData>>()
//.AddSingleton<ILoader<IMetadata>, Loader<IMetadata>>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("model");
    config.ValidateExamples();
    config.SetInterceptor(new LogInterceptor()); // add the interceptor

    config.AddCommand<SettingsCommand>("settings")
        .WithDescription("Manage the settings for the tool")
        .WithExample("settings", "--name", "Settings", "--type", "Json");
    config.AddBranch("list", list =>
    {
        list.SetDescription("List the available definitions or templates");
        list.AddCommand<ListDefinitionsCommand>("definitions")
            .WithExample("list", "definitions", "--folder",
                Path.Combine("c:", "jjs", "set", "Modeller", "src", "modeller.definitions"));
        list.AddCommand<ListTemplatesCommand>("templates")
            .WithExample("list", "templates", "--folder",
                Path.Combine("c:", "jjs", "set", "Modeller", "src", "modeller.templates"), "--target", "net8.0");
    });
    // config.AddCommand<BuildCommand>("build")
    //     .WithDescription("Build a definition using a specific template")
    //     .WithExample("build", "ApiSolution", "CaseDomain", "--definitions", "../src/Modeller.Definitions",
    //         "--templates", "../src/Modeller.Templates", "--output", "c:/playschool");
});

return app.Run(args);