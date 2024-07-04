namespace Domain;

public class Targets
{
    private static readonly IEnumerable<string> SystemTargets = new[] { "net6.0", "net7.0", "net8.0", "net9.0" };
    private List<string> _supportedTargets = [..SystemTargets];

    public Targets()
    {
        Reset();
    }

    public static Targets Shared { get; } = new();

    public static string Default { get; } = SystemTargets.Last();

    public IEnumerable<string> Supported => _supportedTargets;

    public void RegisterTarget(string target)
    {
        if (_supportedTargets.Contains(target))
        {
            return;
        }
        _supportedTargets.Add(target);
    }

    public void Reset()
    {
        _supportedTargets = new(SystemTargets);
    }
}
