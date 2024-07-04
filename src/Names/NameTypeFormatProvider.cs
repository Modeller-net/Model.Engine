namespace Names;

public class NameTypeFormatProvider : IFormatProvider, ICustomFormatter
{
    public object? GetFormat(Type? formatType) =>
        formatType == typeof(ICustomFormatter) ? this : null;

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (arg is not NameType name) return arg?.ToString() ?? "";
        return format switch
        {
            "D" => name.Value.Humanize().Transform(To.TitleCase),
            "L" => name.Value.Dehumanize().Camelize().CheckKeyword(),
            "M" => "_" + name.Value.Dehumanize().Camelize(),
            "S" => name.Value.Dehumanize(),
            _ => name.Value
        };
    }
}
