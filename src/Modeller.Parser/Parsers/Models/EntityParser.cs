using Names;

using static Modeller.Parser;
using static Modeller.Parser<char>;
using static Modeller.Comment.CommentParser;

namespace Modeller.Parsers.Models;

public static class EntityParser
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
    private static readonly Parser<char, string> EntityKeyword = Tok("entity").Labelled("entity keyword");
    private static readonly Parser<char, string> DescriptionKeyword = Tok("description").Labelled("description keyword");

    private static readonly Parser<char, string> FBoolKeyword = Tok("boolean");
    private static readonly Parser<char, string> FStringKeyword = Tok("string");
    private static readonly Parser<char, string> FDateTimeKeyword = Tok("datetime");
    private static readonly Parser<char, string> FDateKeyword = Tok("date");
    private static readonly Parser<char, string> FTimeKeyword = Tok("time");
    private static readonly Parser<char, string> FDateTimeOffsetKeyword = Tok("datetimeoffset");
    private static readonly Parser<char, string> FByteKeyword = Tok("byte");
    private static readonly Parser<char, string> FEnumKeyword = Tok("isEnum");
    private static readonly Parser<char, string> FCurrencyKeyword = Tok("currency");
    private static readonly Parser<char, string> FsEntityKeyword = Tok("isSingle");
    private static readonly Parser<char, string> FmEntityKeyword = Tok("isMultiple");
    private static readonly Parser<char, string> FIntKeyword = Tok("integer");
    private static readonly Parser<char, string> FImageKeyword = Tok("image");
    private static readonly Parser<char, string> FDoubleKeyword = Tok("double");
    private static readonly Parser<char, string> FLatLongKeyword = Tok("latlong");
    private static readonly Parser<char, string> FPercentageKeyword = Tok("percentage");
    
    private static Parser<char, T> Parenthesised<T>(Parser<char, T> parser)
        => parser.Between(LBracket, RBracket);

    private static readonly Parser<char, NameType> NameIdentifier =
        Tok(from first in Letter
            from rest in OneOf(Letter, Digit, Char('_')).ManyString()
            select NameType.FromString(first + rest))
            .Labelled("Name");

    private static readonly Parser<char, AttributeType> KeyValueSyntax =
        from en in Tok(Letter).ManyString()
        from eq in Tok("=").Optional().IgnoreResult()
        from n in LetterOrDigit.ManyString().Optional()
        select new AttributeType(en,n);
    
    private static readonly Parser<char, DataTypeDetail> DataTypeSyntax =
        from name in OneOf(FBoolKeyword, FStringKeyword, FDateTimeKeyword, FDateKeyword,
            FTimeKeyword, FDateTimeOffsetKeyword, FByteKeyword, FEnumKeyword, FCurrencyKeyword, FsEntityKeyword, FmEntityKeyword,
            FIntKeyword, FImageKeyword, FDoubleKeyword, FLatLongKeyword, FPercentageKeyword)
        from attrs in KeyValueSyntax.Separated(Comma).Between(LBracket, RBracket)
        select new DataTypeDetail(name, attrs);

    private static readonly Parser<char, string> VersionSyntax =
        Try(
            LetterOrDigit.ManyString().Before(Period).Select(s => s)
        ).Labelled("Version");

    internal static readonly Parser<char, VersionedEntityName> EntitySyntax =
        from kw in EntityKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedEntityName(n, v);

    private static readonly Parser<char, string> ClearText =
        Token(c => c != '"')
            .ManyString()
            .Between(Quote);

    internal static readonly Parser<char, NonEmptyString> DescriptionSyntax =
        DescriptionKeyword
            .Then(Parenthesised(ClearText.Select(s => new NonEmptyString(s))))
            .Labelled("'description(\"<clear text description>\")'");

    private static readonly Parser<char, Unit> SkipComment = SkipWhitespaces
        .Then(Try(SkipLineComment(Char('#'))
            .SeparatedAndOptionallyTerminated(SkipWhitespaces)
            .IgnoreResult()))
        .Before(SkipWhitespaces);

    internal static readonly Parser<char, FieldDetail> FieldSyntax =
        from n in NameIdentifier
        from colon in Colon
        from dt in DataTypeSyntax 
        from comma in Comma.Labelled("Comma")
        from s in DescriptionSyntax
        select new FieldDetail(n, dt, s);
    
    private static readonly Parser<char, EntityBuilder> EntityParserRule =
        from c in SkipComment
        from en in EntitySyntax
        from co in Colon
        from desc in DescriptionSyntax
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new EntityBuilder(en, desc, fields);

    public static EntityBuilder Parse(string input)
        => EntityParserRule.ParseOrThrow(input);
}
