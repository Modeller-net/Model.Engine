namespace Names;

internal enum LetterCasing
{
    Title,
    /// <summary>
    /// SomeString -> SOME STRING
    /// </summary>
    AllCaps,
    /// <summary>
    /// SomeString -> some string
    /// </summary>
    LowerCase,
    /// <summary>
    /// SomeString -> Some string
    /// </summary>
    Sentence,
}