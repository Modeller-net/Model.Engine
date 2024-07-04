namespace Domain;

/// <summary>
/// Represents a file version.
/// </summary>
public class FileVersion : IEquatable<Version>, IComparable, IComparable<Version>, IComparable<FileVersion>
{
    private int _preRelease;
    private const int Released = 100;
    private const int LessThan = -1;
    private const int GreaterThan = 1;
    private const int Equal = 0;
    private const int Alpha = 1;
    private const int Beta = 2;

    public FileVersion()
        : this(string.Empty)
    { }


    private FileVersion(Version version)
        : this(version.ToString())
    { }

    public FileVersion(string? version)
    {
        ForceVersion(version);
    }

    public Version Version { get; set; } = new();

    private void ForceVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            Version = new();
            return;
        }

        var sep = version.IndexOf('-', StringComparison.InvariantCulture);
        if (sep > -1)
        {
            if (version.EndsWith("-alpha"))
            {
                IsAlphaRelease = true;
            }
            else
            {
                IsBetaRelease = version.EndsWith("-beta")
                    ? true
                    : throw new FormatException("Version was not the correct format. Use: major.minor.revision.build[-alpha|-beta]");
            }

            version = version.Remove(sep);
        }
        Version = Version.TryParse(version, out var result)
            ? result
            : throw new FormatException("Version was not the correct format. Use: major.minor.revision.build[-alpha|-beta]");
    }

    public bool IsRelease
    {
        get => _preRelease is Released or 0;
        set
        {
            if (value)
            {
                _preRelease = Released;
            }
        }
    }
    public bool IsAlphaRelease
    {
        get => _preRelease == Alpha;
        set
        {
            if (value)
            {
                _preRelease = Alpha;
            }
        }
    }

    public bool IsBetaRelease
    {
        get => _preRelease == Beta;
        set
        {
            if (value)
            {
                _preRelease = Beta;
            }
        }
    }

    public static FileVersion Initial => new("1.0");
    public static FileVersion Empty => new("0.0");

    public override string ToString()
    {
        var s = Version.ToString();
        if (IsAlphaRelease)
        {
            s += "-alpha";
        }
        else if (IsBetaRelease)
        {
            s += "-beta";
        }

        return s;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            null => false,
            FileVersion gv => CompareTo(gv) == Equal,
            Version v => CompareTo(v) == Equal,
            _ => false
        };
    }

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);

    public bool Equals(Version? other) => other is not null && CompareTo(new FileVersion(other)) == Equal;

    private static int PreRelease(FileVersion v) => v.IsAlphaRelease ? Alpha : v.IsBetaRelease ? Beta : Released;

    public static bool operator ==(FileVersion v1, FileVersion v2) => v1.Equals(v2);

    public static bool operator !=(FileVersion v1, FileVersion v2) => !v1.Equals(v2);

    public static bool operator >(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) == GreaterThan;

    public static bool operator <(FileVersion v1, FileVersion v2) => v1.CompareTo(v2) == LessThan;

    public static bool operator >=(FileVersion v1, FileVersion v2)
    {
        return v1.Equals(v2) || v1 > v2;
    }

    public static bool operator <=(FileVersion v1, FileVersion v2)
    {
        return v1.Equals(v2) || v1 < v2;
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Version v => CompareTo(v),
            FileVersion gv => CompareTo(gv),
            string s => CompareTo(new FileVersion(s)),
            _ => throw new InvalidCastException(
                $"Unable to cast {obj?.GetType().FullName} to a {typeof(FileVersion).FullName}")
        };
    }

    public int CompareTo(Version? other)
    {
        var result = Version.CompareTo(other);
        return result == Equal && (IsAlphaRelease || IsBetaRelease) ? LessThan : result;
    }

    public int CompareTo(FileVersion? other)
    {
        if (other is null) return 1;
        var result = Version.CompareTo(other.Version);
        if (result != Equal)
        {
            return result;
        }

        var p1 = PreRelease(this);
        var p2 = PreRelease(other);
        return p1 == p2 ? Equal : p1 > p2 ? GreaterThan : LessThan;
    }
}