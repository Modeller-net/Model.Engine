namespace Names;

public static class StringExtensions
{
    public static string ToPlural(this string value) => value.Pluralize(false);

    public static string ToSingular(this string value) => value.Singularize(false);
}