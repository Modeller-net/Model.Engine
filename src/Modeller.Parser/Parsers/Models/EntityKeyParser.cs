using Names;

using static Modeller.Parser;
using static Modeller.Parser<char>;
using static Modeller.Comment.CommentParser;

namespace Modeller.Parsers.Models;

public static class EntityKeyParser
{
    private static Parser<char, T> Tok<T>(Parser<char, T> p) => Try(p).Before(SkipWhitespaces);

    private static Parser<char, char> Tok(char value) => Tok(Char(value));

    private static Parser<char, string> Tok(string value) => Tok(String(value));

    private static readonly Parser<char, char> LBracket = Tok('(');
    private static readonly Parser<char, char> RBracket = Tok(')');
    private static readonly Parser<char, char> LBrace = Tok('{');
    private static readonly Parser<char, char> RBrace = Tok('}');
    private static readonly Parser<char, char> Colon = Tok(':');
    private static readonly Parser<char, char> Comma = Tok(',');
    private static readonly Parser<char, char> Period = Tok('.');
    private static readonly Parser<char, char> Quote = Tok('"');
    private static readonly Parser<char, string> EntityKeyKeyword = Tok("key").Labelled("key keyword");

    private static readonly Parser<char, string>
        DescriptionKeyword = Tok("description").Labelled("description keyword");

    private static Parser<char, T> Parenthesised<T>(Parser<char, T> parser)
        => parser.Between(LBracket, RBracket);

    private static readonly Parser<char, NameType> NameIdentifier =
        Tok(from first in Letter
                from rest in OneOf(Letter, Digit, Char('_')).ManyString()
                select NameType.FromString(first + rest))
            .Labelled("Name");

    private static readonly Parser<char, string> VersionSyntax =
        Try(
            LetterOrDigit.ManyString().Before(Period).Select(s => s)
        ).Labelled("Version");

    internal static readonly Parser<char, string> ClearText =
        Token(c => c != '"')
            .ManyString()
            .Between(Quote);
    
    internal static readonly Parser<char, VersionedEntityName> EntityKeySyntax =
        from kw in EntityKeyKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedEntityName(n, v);

    internal static readonly Parser<char, NonEmptyString> DescriptionSyntax =
        DescriptionKeyword
            .Then(Parenthesised(ClearText.Select(s => new NonEmptyString(s))))
            .Labelled("'description(\"<clear text description>\")'");

    private static readonly Parser<char, Unit> SkipComment = SkipWhitespaces
        .Then(Try(SkipLineComment(Char('#'))
            .SeparatedAndOptionallyTerminated(SkipWhitespaces)
            .IgnoreResult()))
        .Before(SkipWhitespaces);

    private static readonly Parser<char, EntityKeyBuilder> EntityKeyParserRule =
        from c in SkipComment
        from en in EntityKeySyntax
        from co in Colon
        from desc in DescriptionSyntax
        //from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new EntityKeyBuilder(en, desc);
    
    public static EntityKeyBuilder Parse(string input)
        => EntityKeyParserRule.ParseOrThrow(input);
}

public record EntityKeyBuilder(VersionedEntityName Name, NonEmptyString Summary);
