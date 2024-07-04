namespace Names;

/// <summary>
/// ApplyCase method to allow changing the case of a sentence easily
/// </summary>
internal static class CasingExtensions
{
    /// <summary>
    /// Changes the casing of the provided input
    /// </summary>
    /// <param name="input"></param>
    /// <param name="casing"></param>
    /// <returns></returns>
    public static string ApplyCase(this string input, LetterCasing casing)
    {
        return casing switch
        {
            LetterCasing.Title => input.Transform(To.TitleCase),
            LetterCasing.LowerCase => input.Transform(To.LowerCase),
            LetterCasing.AllCaps => input.Transform(To.UpperCase),
            LetterCasing.Sentence => input.Transform(To.SentenceCase),
            _ => throw new ArgumentOutOfRangeException(nameof(casing))
        };
    }
}