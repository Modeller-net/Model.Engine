namespace Model.Domain;

internal static class Constants
{
    public const string ToolName = "Model";
    public const string SolutionName = "Modeller.Net";
    public const string DefaultTemplateFolder = "Templates";
    public const string DefaultDefinitionFolder = "Definitions";
    public const string DefaultOutputFolder = "Output";
    public const string DefaultFilename = nameof(Settings);
}

/// <summary>
/// Provides utility methods for development purposes.
/// </summary>
internal static class DevUtilities
{
    private static IDebuggerWrapper _debuggerWrapper = new DebuggerWrapper();

    internal class DebuggerWrapper : IDebuggerWrapper
    {
        private static readonly IFileSystem _fileSystem = new FileSystem();
        public bool IsAttached => Debugger.IsAttached;
        public string ExecutingAssemblyLocation => Assembly.GetExecutingAssembly().Location;
        public IFileSystem FileSystem => _fileSystem;
    }

    internal static void SetDebuggerWrapper(IDebuggerWrapper debuggerWrapper)
    {
        _debuggerWrapper = debuggerWrapper;
    }
    
    /// <summary>
    /// Gets the solution directory based on the provided folder.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the application is running with a debugger attached, the solution directory is determined by navigating up
    /// the directory tree from the 'ExecutingAssembly location until the solution file is found.
    /// </para>
    /// <para>
    /// If the application is running in release mode, the solution directory is determined by combining the
    ///  <see cref="Environment.SpecialFolder.ApplicationData"/> directory with the <c>Model</c> folder. 
    /// </para>
    /// </remarks>
    /// <param name="folder">The folder to combine with the solution directory path.</param>
    /// <returns>A <see cref="DirectoryInfo"/> object representing the solution directory.</returns>
    public static DirectoryInfo GetSolutionDirectory(string folder)
    {
        var assemblyDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);
        if (_debuggerWrapper.IsAttached)
        {
            // Get the base directory of the executing assembly
            assemblyDirectory = Path.GetDirectoryName(_debuggerWrapper.ExecutingAssemblyLocation);
    
            // If running from Visual Studio, navigate up to the solution directory
            while (!string.IsNullOrEmpty(assemblyDirectory) &&
                   !_debuggerWrapper.FileSystem.File.Exists(Path.Combine(assemblyDirectory, $"{Constants.SolutionName}.sln")))
            {
                assemblyDirectory = Path.GetDirectoryName(assemblyDirectory);
            }
        }
        else
        {
            assemblyDirectory = Path.Combine(assemblyDirectory, Constants.ToolName);
        }
    
        return new(string.IsNullOrWhiteSpace(assemblyDirectory) 
            ? folder 
            : Path.Combine(assemblyDirectory, folder));
    }
}

public interface IDebuggerWrapper
{
    bool IsAttached { get; }
    string ExecutingAssemblyLocation { get; }
    IFileSystem FileSystem { get; }
}
