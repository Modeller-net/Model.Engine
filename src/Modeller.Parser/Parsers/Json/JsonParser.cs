namespace Modeller.Parsers;

public static class JsonParser
{
    private static readonly Parser<char, char> LBrace = Parser.Char('{');
    private static readonly Parser<char, char> RBrace = Parser.Char('}');
    private static readonly Parser<char, char> LBracket = Parser.Char('[');
    private static readonly Parser<char, char> RBracket = Parser.Char(']');
    private static readonly Parser<char, char> Quote = Parser.Char('"');
    private static readonly Parser<char, char> Colon = Parser.Char(':');
    private static readonly Parser<char, char> ColonWhitespace =
        Colon.Between(Parser.SkipWhitespaces);

    private static readonly Parser<char, char> Comma = Parser.Char(',');

    private static readonly Parser<char, string> String =
        Parser<char>.Token(c => c != '"')
            .ManyString()
            .Between(Quote);

    private static readonly Parser<char, Json> JsonString =
        String.Select<Json>(s => new JsonString(s));

    private static readonly Parser<char, Json> Json =
        JsonString.Or(Parser.Rec(() => JsonArray!)).Or(Parser.Rec(() => JsonObject!));

    private static readonly Parser<char, Json> JsonArray =
        Json.Between(Parser.SkipWhitespaces)
            .Separated(Comma)
            .Between(LBracket, RBracket)
            .Select<Json>(els => new JsonArray([..els]));

    private static readonly Parser<char, KeyValuePair<string, Json>> JsonMember =
        String
            .Before(ColonWhitespace)
            .Then(Json, (name, val) => new KeyValuePair<string, Json>(name, val));

    private static readonly Parser<char, Json> JsonObject =
        JsonMember.Between(Parser.SkipWhitespaces)
            .Separated(Comma)
            .Between(LBrace, RBrace)
            .Select<Json>(kvps => new JsonObject(kvps.ToImmutableDictionary()));

    public static Result<char, Json> Parse(string input) => Json.Parse(input);
}
