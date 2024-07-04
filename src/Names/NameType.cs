namespace Names;

/// <summary>
/// Creates a name using the following rules. Removes spaces and capitalizes each word.
/// </summary>
/// <param name="Value">The value to make into a name</param>
/// <param name="MakeSingle">If makeSingle is true (default), the value is transformed to a singular form</param>
/// <returns></returns>
// ReSharper disable once NotAccessedPositionalProperty.Global
public readonly record struct NameType(string Value, bool MakeSingle = true)
{
    private static readonly IFormatProvider DefaultFormatProvider = new NameTypeFormatProvider();

    public string Value { get; } = ProcessValue(Value, MakeSingle);

    private static string ProcessValue(string value, bool makeSingle)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A NameType cannot be null or whitespace.", nameof(value));

        value = value.Dehumanize();
        return makeSingle ? value.Singularize(false) : value;
    }

    public static implicit operator string(NameType value) => value.ToString();

    public static NameType FromString(string value, bool makeSingle = true) => new(value, makeSingle);

    public override string ToString() => ToString("", DefaultFormatProvider);

    /// <summary>
    /// Converts the value of the current NameType object to its equivalent string representation
    /// using the specified format and format provider.
    /// </summary>
    /// <param name="format">
    /// A standard format string to specify the formatting style. The following options are available:
    /// <list type="bullet">
    /// <item>
    /// <term>D</term>
    /// <description> Display style: Converts the name to a human-readable display format with title case.</description>
    /// </item>
    /// <item>
    /// <term>L</term>
    /// <description> LocalVariable style: Converts the name to a camel case format suitable for local variables.</description>
    /// </item>
    /// <item>
    /// <term>M</term>
    /// <description> ModularVariable style: Converts the name to a camel case format prefixed with an underscore.</description>
    /// </item>
    /// <item>
    /// <term>S</term>
    /// <description> StaticVariable style: Dehumanizes the name without further transformations.</description>
    /// </item>
    /// <item>
    /// <term>null or empty</term>
    /// <description>Default style: Returns the default value.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="formatProvider">An optional format provider to use for formatting. If null, the default format provider is used.</param>
    /// <returns>A string representation of the current NameType object, formatted as specified.</returns>
    public string ToString(string format, IFormatProvider? formatProvider = null)
    {
        formatProvider ??= DefaultFormatProvider;
        return string.Format(formatProvider, $"{{0:{format}}}", this);
    }
}