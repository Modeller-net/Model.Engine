namespace Model.Domain;

/// <summary>
/// Provides extension methods for working with target frameworks.
/// </summary>
public static class TargetExtensions
{
    /// <summary>
    /// Determines if a target framework is out of support based on the provided date.
    /// </summary>
    public static bool IsOutOfSupport(this Target target, DateOnly referenceDate)
        => referenceDate > target.OutOfSupport;

    /// <summary>
    /// Filters supported targets by removing those out of support based on the provided date.
    /// </summary>
    public static IEnumerable<Target> FilterSupportedTargets(this IEnumerable<Target> targets, DateOnly referenceDate)
        => targets.Where(t => !t.IsOutOfSupport(referenceDate));
}

/// <summary>
/// Represents a target framework and its out-of-support date.
/// </summary>
public readonly record struct Target(string Value, DateOnly OutOfSupport)
{
    public string Value { get; } = Value.ToLowerInvariant();
}

/// <summary>
/// Manages a list of supported target frameworks.
/// </summary>
public sealed class Targets
{
    private static readonly ImmutableList<Target> DefaultTargets = ImmutableList.Create(
        new Target("net5.0", new DateOnly(2022, 5, 10)),
        new Target("net6.0", new DateOnly(2024, 11, 12)),
        new Target("net7.0", new DateOnly(2024, 5, 14)),
        new Target("net8.0", new DateOnly(2026, 11, 10)),
        new Target("net9.0", new DateOnly(2026, 5, 12))
    );

    public Targets()
        : this(DefaultTargets) { }

    private Targets(ImmutableList<Target> supportedTargets)
    {
        SupportedTargets = supportedTargets;
    }
    
    /// <summary>
    /// Gets the currently supported target frameworks.
    /// </summary>
    public ImmutableList<Target> SupportedTargets { get; }
    
    /// <summary>
    /// Gets the default target framework.
    /// </summary>
    public static string Default { get; } = DefaultTargets.Last().Value;

    /// <summary>
    /// Creates a new instance of Targets with the specified target registered, if it doesn't already exist.
    /// </summary>
    public Targets WithRegisteredTarget(Target target)
    {
        return SupportedTargets.Any(t => string.Equals(t.Value, target.Value, StringComparison.OrdinalIgnoreCase)) 
            ? this // Return the same instance if the target already exists 
            : new Targets(SupportedTargets.Add(target));
    }

    /// <summary>
    /// Resets the supported targets to the default system targets.
    /// </summary>
    public Targets Reset()
    {
        return new Targets(DefaultTargets);
    }
}