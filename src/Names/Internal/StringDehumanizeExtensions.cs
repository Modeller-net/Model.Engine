namespace Names;

/// <summary>
/// Contains extension methods for dehumanizing strings.
/// </summary>
internal static class StringDehumanizeExtensions
{
    /// <summary>
    /// Dehumanizes a string; e.g. 'some string', 'Some String', 'Some string' -> 'SomeString'
    /// If a string is already dehumanized then it leaves it alone 'SomeStringAndAnotherString' -> 'SomeStringAndAnotherString'
    /// </summary>
    /// <param name="input">The string to be dehumanized</param>
    /// <returns></returns>
    public static string Dehumanize(this string input) => 
        string.Join("", input.Split(' ').Select(word => word.Humanize().Pascalize())).Replace(" ", "");
}