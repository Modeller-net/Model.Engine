namespace Model.Contracts;


/// <summary>
/// Represents a file version with optional pre-release tags (alpha, beta, or release).
/// </summary>
public readonly record struct FileVersion(string Version, PreReleaseType PreRelease = PreReleaseType.Release) : IComparable<FileVersion>
{
    public static FileVersion Empty => new("0.0");

    /// <summary>
    /// Parses a version string and creates a <see cref="FileVersion"/>.
    /// </summary>
    public static FileVersion Parse(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return Empty;
        }

        // Split by the first '-' to separate version and pre-release tag
        var sepIndex = version.IndexOf('-', StringComparison.InvariantCulture);
        if (sepIndex > -1)
        {
            var baseVersion = version.Substring(0, sepIndex);
            var preReleaseTag = version.Substring(sepIndex + 1);

            var preRelease = preReleaseTag.ToLowerInvariant() switch
            {
                "alpha" => PreReleaseType.Alpha,
                "beta" => PreReleaseType.Beta,
                _ => throw new FormatException("Invalid pre-release tag. Use: -alpha or -beta.")
            };

            return new FileVersion(baseVersion, preRelease);
        }

        if (!System.Version.TryParse(version, out _))
        {
            throw new FormatException("Invalid version format. Use: major.minor.revision.build[-alpha|-beta].");
        }

        return new FileVersion(version);
    }

    /// <summary>
    /// Returns the version as a string with its pre-release tag.
    /// </summary>
    public override string ToString() =>
        PreRelease switch
        {
            PreReleaseType.Alpha => $"{Version}-alpha",
            PreReleaseType.Beta => $"{Version}-beta",
            _ => Version
        };

    /// <summary>
    /// Compares this version to another version.
    /// </summary>
    public int CompareTo(FileVersion other)
    {
        var baseComparison = string.Compare(Version, other.Version, StringComparison.Ordinal);
        if (baseComparison != 0) return baseComparison;

        return PreRelease.CompareTo(other.PreRelease);
    }

    public static bool operator >(FileVersion left, FileVersion right) => left.CompareTo(right) > 0;
    public static bool operator <(FileVersion left, FileVersion right) => left.CompareTo(right) < 0;
    public static bool operator >=(FileVersion left, FileVersion right) => left.CompareTo(right) >= 0;
    public static bool operator <=(FileVersion left, FileVersion right) => left.CompareTo(right) <= 0;
}

/// <summary>
/// Represents the type of pre-release for a file version.
/// </summary>
public enum PreReleaseType
{
    Release = 100,
    Alpha = 1,
    Beta = 2
}