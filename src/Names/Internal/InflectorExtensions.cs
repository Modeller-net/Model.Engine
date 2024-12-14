using System.Text.RegularExpressions;

namespace Names;

/// <summary>
/// Inflector extensions
/// </summary>
internal static partial class InflectorExtensions
{
    /// <summary>
    /// Pluralizes the provided input considering irregular words
    /// </summary>
    /// <param name="word">Word to be pluralized</param>
    /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
    /// <returns></returns>
    public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true) => Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);

    /// <summary>
    /// Singularizes the provided input considering irregular words
    /// </summary>
    /// <param name="word">Word to be singularized</param>
    /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
    /// <param name="skipSimpleWords">Skip singularizing single words that have an 's' on the end</param>
    /// <returns></returns>
    public static string Singularize(this string word, bool inputIsKnownToBePlural = true, bool skipSimpleWords = false) => Vocabularies.Default.Singularize(word, inputIsKnownToBePlural, skipSimpleWords);

    /// <summary>
    /// Humanizes the input with Title casing
    /// </summary>
    /// <param name="input">The string to be titleized</param>
    /// <returns></returns>
    public static string Titleize(this string input) => input.Humanize(LetterCasing.Title);

    /// <summary>
    /// By default, pascalize converts strings to UpperCamelCase also removing underscores
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Pascalize(this string input) => MyRegex().Replace(input, match => match.Groups[1].Value.ToUpper());

    /// <summary>
    /// Same as Pascalize except that the first character is lower case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Camelize(this string input)
    {
        var word = input.Pascalize();
        return word.Length > 0 ? word[..1].ToLower() + word[1..] : word;
    }

    /// <summary>
    /// Separates the input words with underscore
    /// </summary>
    /// <param name="input">The string to be underscored</param>
    /// <returns></returns>
    public static string Underscore(this string input) => MyRegex3().Replace(MyRegex2().Replace(MyRegex1().Replace(input, "$1_$2"), "$1_$2"), "_").ToLower();

    /// <summary>
    /// Replaces underscores with hyphens in the string
    /// </summary>
    /// <param name="underscoredWord"></param>
    /// <returns></returns>
    public static string Hyphenate(this string underscoredWord) => underscoredWord.Replace('_', '-');

    /// <summary>
    /// Separates the input words with hyphens and all the words are converted to lowercase
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Kebaberize(this string input) => Underscore(input).Hyphenate();

    [GeneratedRegex("(?:^|_| +)(.)")]
    private static partial Regex MyRegex();
    
    [GeneratedRegex(@"([\p{Lu}]+)([\p{Lu}][\p{Ll}])")]
    private static partial Regex MyRegex1();
    
    [GeneratedRegex(@"([\p{Ll}\d])([\p{Lu}])")]
    private static partial Regex MyRegex2();
    
    [GeneratedRegex(@"[-\s]")]
    private static partial Regex MyRegex3();
}