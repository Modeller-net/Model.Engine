namespace Domain;

public record Settings(string TemplateFolder, string DefinitionFolder, string OutputFolder, string Target)
{
    [IgnoreDataMember]
    [JsonIgnore]
    public Enterprise? Enterprise { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public Service? DomainService { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public EntityType? Entity { get; set; }

    public string? TemplateName { get; init; }

    public string? DefinitionName { get; init; }

    public bool SupportRegen { get; init; } = true;

    public string Version { get; init; } = "1.0.0";

    public IList<Package> Packages { get; init; } = [];

    public static Settings Default => new(
        DefaultTemplateFolder.FullName,
        DefaultDefinitionFolder.FullName,
        DefaultOutputFolder.FullName,
        Targets.Default);

    public static System.IO.DirectoryInfo DefaultTemplateFolder => new(GetSolutionDirectory("Generators"));

    public static System.IO.DirectoryInfo DefaultDefinitionFolder => new(GetSolutionDirectory("Definitions"));

    public static System.IO.DirectoryInfo DefaultOutputFolder => new(GetSolutionDirectory("Output"));

    private static string GetSolutionDirectory(string folder)
    {
        var assemblyDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
        if (Debugger.IsAttached)
        {
            // Get the base directory of the executing assembly
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyLocation);

            // If running from Visual Studio, navigate up to the solution directory
            while (!string.IsNullOrEmpty(assemblyDirectory) && !System.IO.File.Exists(Path.Combine(assemblyDirectory, "Modeller.sln")))
            {
                assemblyDirectory = Path.GetDirectoryName(assemblyDirectory);
            }
        }
        else
        {
            assemblyDirectory = Path.Combine(assemblyDirectory, "Modeller");
        }

        return string.IsNullOrWhiteSpace(assemblyDirectory) ? folder : Path.Combine(assemblyDirectory, folder);
    }
}
