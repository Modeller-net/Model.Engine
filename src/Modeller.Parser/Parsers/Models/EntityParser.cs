﻿using Names;
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
    private static readonly Parser<char, string> EndpointKeyword = Tok("endpoint").Labelled("endpoint keyword");
    private static readonly Parser<char, string> EnumKeyword = Tok("enum").Labelled("enum keyword");
    private static readonly Parser<char, string> FlagKeyword = Tok("flags").Labelled("flag keyword");

    private static readonly Parser<char, string>
        DescriptionKeyword = Tok("description").Labelled("description keyword");

    private static readonly Parser<char, string> EntityKeyKeyword = Tok("key").Labelled("key keyword");
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

    private static readonly Parser<char, string> GetKeyword = Tok("Get");
    private static readonly Parser<char, string> PostKeyword = Tok("post");
    private static readonly Parser<char, string> PutKeyword = Tok("put");
    private static readonly Parser<char, string> DeleteKeyword = Tok("delete");

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
        select new AttributeType(en, n);

    private static readonly Parser<char, DataTypeDetail> DataTypeSyntax =
        from name in OneOf(FBoolKeyword, FStringKeyword, FDateTimeKeyword, FDateKeyword,
            FTimeKeyword, FDateTimeOffsetKeyword, FByteKeyword, FEnumKeyword, FCurrencyKeyword, FsEntityKeyword,
            FmEntityKeyword,
            FIntKeyword, FImageKeyword, FDoubleKeyword, FLatLongKeyword, FPercentageKeyword)
        from attrs in KeyValueSyntax.Separated(Comma).Between(LBracket, RBracket)
        select new DataTypeDetail(name, attrs);

    private static readonly Parser<char, string> VersionSyntax =
        Try(
            LetterOrDigit.ManyString().Before(Period).Select(s => s)
        ).Labelled("Version");

    internal static readonly Parser<char, VersionedName> EntitySyntax =
        from kw in EntityKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> EndpointSyntax =
        from kw in EndpointKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> EnumSyntax =
        from kw in EnumKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> FlagSyntax =
        from kw in FlagKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

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
    
    internal static readonly Parser<char, EnumDetail> EnumValueSyntax =
        from n in NameIdentifier
        from colon in Colon
        from v in UnsignedInt(10).Before(Comma)
        from s in DescriptionSyntax
        select new EnumDetail(n, v, s);

    internal static readonly Parser<char, FlagDetail> FlagValueSyntax =
        from n in NameIdentifier
        from colon in Colon
        from v in UnsignedInt(10).Before(Comma)
        from s in DescriptionSyntax
        select new FlagDetail(n, v, s);

    private static readonly Parser<char, string> UsesOwnerKeyword = Tok("usesOwnerKey").Labelled("usesOwnerKey keyword");
    private static readonly Parser<char, string> OwnerKeyword = Tok("owner").Labelled("owner keyword");
    private static readonly Parser<char, string> OperationKeyword = Tok("operation").Labelled("operation keyword");
    private static readonly Parser<char, string> UntenantedKeyword = Tok("untenanted").Labelled("untenanted keyword");
    private static readonly Parser<char, string> PathKeyword = Tok("path").Labelled("path keyword");

    private static readonly Parser<char, string> TenantKeyword =
        Try(
            Comma.Then(Tok("tenantkey").Select(_ => "tenantkey"))
        ).Labelled("Tenant");

    internal static readonly Parser<char, VersionedName> EntityKeySyntax =
        from kw in EntityKeyKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    private static readonly Parser<char, OwnerKeyType> KeyOwnerSyntax =
        from kw in OneOf(OwnerKeyword, UsesOwnerKeyword, UntenantedKeyword).ManyString()
        from n in Parenthesised(NameIdentifier).Optional()
        select n.HasValue ? new OwnerKeyType(kw, n.Value) : new OwnerKeyType(kw);

    private static readonly Parser<char, EntityKeyBuilder> EntityKeyParserRule =
        from c in SkipComment
        from en in EntityKeySyntax
        from co in Colon
        from k in KeyOwnerSyntax.Before(Comma).Optional()
        from desc in DescriptionSyntax
        from kt in TenantKeyword.Optional()
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new EntityKeyBuilder(en, k, kt.HasValue, desc, fields);

    private static readonly Parser<char, EntityBuilder> EntityParserRule =
        from c in SkipComment
        from en in EntitySyntax
        from co in Colon
        from desc in DescriptionSyntax
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new EntityBuilder(en, desc, fields);

    private static readonly Parser<char, OwnerDetail> OwnerSyntax =
        from kw in OwnerKeyword
        from n in Parenthesised(NameIdentifier)
        select new OwnerDetail(n);
    
    private static readonly Parser<char, string> OperationSyntax =
        from kw in OperationKeyword
        from op in Parenthesised(OneOf(GetKeyword, PostKeyword, PutKeyword, DeleteKeyword).Between(Quote)).Labelled("Operation Type")
        select op;

    private static readonly Parser<char, string> PathSyntax =
        PathKeyword
            .Then(Parenthesised(ClearText.Select(s => s)))
            .Labelled("'path(\"<path>\")'");
    
    private static readonly Parser<char, EndpointBodyDetail> EndpointBodySyntax =
        from op in OperationSyntax.Before(Comma)
        from pa in PathSyntax.Before(Comma)
        // from qp in QueryParamsSyntax
        // from rs in ResponseSyntax
        select new EndpointBodyDetail(op, pa);
    
    private static readonly Parser<char, EndpointBuilder> EndpointParserRule =
        from c in SkipComment
        from ep in EndpointSyntax
        from co in Colon
        from o in OwnerSyntax.Before(Comma)
        from desc in DescriptionSyntax
        from body in EndpointBodySyntax.Between(LBrace, RBrace)
        select new EndpointBuilder(ep,o,desc);
    
    private static readonly Parser<char, EnumBuilder> EnumParserRule =
        from c in SkipComment
        from en in EnumSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from values in EnumValueSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new EnumBuilder(en, desc, values);

    private static readonly Parser<char, FlagBuilder> FlagParserRule =
        from c in SkipComment
        from en in FlagSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from values in FlagValueSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(LBrace, RBrace)
        select new FlagBuilder(en, desc, values);

    public static readonly Func<string, Builder> ParseEntity = input => EntityParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEntityKey= input => EntityKeyParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEnum = input => EnumParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseFlag = input => FlagParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEndpoint = input => EndpointParserRule.ParseOrThrow(input);
}