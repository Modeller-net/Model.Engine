namespace Names;

internal static class KeywordExtensions
{
    /// <summary>
    /// Reserved words in C#.
    /// </summary>
    public static readonly IEnumerable<string> ReservedWords = new[]
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "required", "sbyte", "scoped", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
        "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "and", "alias",
        "ascending", "args", "async", "await", "by", "descending", "dynamic", "equals", "file", "from", "get", "global",
        "group", "init", "into", "join", "let", "managed", "nameof", "nint", "not", "notnull", "nuint", "on", "or",
        "orderby", "partial", "record", "remove", "required", "scoped", "select", "set", "unmanaged", "value", "var",
        "when", "where", "with", "yield"
    };
    
    /// <summary>
    /// Checks if the given value is a reserved word and if so, prepends it with an '@' symbol.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string CheckKeyword(this string value) => ReservedWords.Contains(value) ? "@" + value : value;
}