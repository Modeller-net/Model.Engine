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
    
    private static readonly Parser<char, char> BracketLeft = Tok('(');
    private static readonly Parser<char, char> BracketRight = Tok(')');
    private static readonly Parser<char, char> BraceLeft = Tok('{');
    private static readonly Parser<char, char> BraceRight = Tok('}');
    private static readonly Parser<char, char> Colon = Tok(':');
    private static readonly Parser<char, char> Comma = Tok(',');
    private static readonly Parser<char, char> Period = Tok('.');
    private static readonly Parser<char, char> Quote = Tok('"');

    private static readonly Parser<char, string> ObjectEntityKeyword = Tok("entity").Labelled("entity keyword");
    private static readonly Parser<char, string> ObjectRpcTypeKeyword = Tok("rpc_type").Labelled("rpc type keyword");
    private static readonly Parser<char, string> ObjectDomainKeyword = Tok("domain").Labelled("domain keyword");
    private static readonly Parser<char, string> ObjectEndpointKeyword = Tok("endpoint").Labelled("endpoint keyword");
    private static readonly Parser<char, string> ObjectEnumKeyword = Tok("enum").Labelled("enum keyword");
    private static readonly Parser<char, string> ObjectFlagKeyword = Tok("flags").Labelled("flag keyword");
    private static readonly Parser<char, string> ObjectServiceKeyword = Tok("service").Labelled("service keyword");
    private static readonly Parser<char, string> ObjectEntityKeyKeyword = Tok("key").Labelled("key keyword");
    private static readonly Parser<char, string> ObjectRpcKeyword = Tok("rpc").Labelled("rpc keyword");

    private static readonly Parser<char, string> ServiceImplementsRpcsKeyword = Tok("implements_rpcs").Labelled("implements_rpcs keyword");
    private static readonly Parser<char, string> ServiceEntitiesKeyword = Tok("entities").Labelled("entities keyword");
    private static readonly Parser<char, string> ServiceCallsRpcsKeyword = Tok("calls_rpcs").Labelled("calls_rpcs keyword");
    private static readonly Parser<char, string> ServiceEnumsKeyword = Tok("enums").Labelled("enums keyword");
    private static readonly Parser<char, string> ServiceReferencesKeyword = Tok("references").Labelled("references keyword");

    private static readonly Parser<char, string> AttributeDescriptionKeyword = Tok("description").Labelled("description keyword");

    private static readonly Parser<char, string> DataTypeBoolKeyword = Tok("boolean");
    private static readonly Parser<char, string> DataTypeStringKeyword = Tok("string");
    private static readonly Parser<char, string> DataTypeDateTimeKeyword = Tok("datetime");
    private static readonly Parser<char, string> DataTypeDateKeyword = Tok("date");
    private static readonly Parser<char, string> DataTypeTimeKeyword = Tok("time");
    private static readonly Parser<char, string> DataTypeDateTimeOffsetKeyword = Tok("datetimeoffset");
    private static readonly Parser<char, string> DataTypeByteKeyword = Tok("byte");
    private static readonly Parser<char, string> DataTypeEnumKeyword = Tok("isEnum");
    private static readonly Parser<char, string> DataTypeCurrencyKeyword = Tok("currency");
    private static readonly Parser<char, string> DataTypeSingleEntityKeyword = Tok("isSingle");
    private static readonly Parser<char, string> DataTypeMultipleEntityKeyword = Tok("isMultiple");
    private static readonly Parser<char, string> DataTypeIntKeyword = Tok("integer");
    private static readonly Parser<char, string> DataTypeLongKeyword = Tok("long");
    private static readonly Parser<char, string> DataTypeULongKeyword = Tok("ulong");
    private static readonly Parser<char, string> DataTypeDocumentKeyword = Tok("document");
    private static readonly Parser<char, string> DataTypeImageKeyword = Tok("image");
    private static readonly Parser<char, string> DataTypeDoubleKeyword = Tok("double");
    private static readonly Parser<char, string> DataTypeLatLongKeyword = Tok("latlong");
    private static readonly Parser<char, string> DataTypePercentageKeyword = Tok("percentage");
    private static readonly Parser<char, string> DataTypeRpcTypeKeyword = Tok("isRPCType");
    private static readonly Parser<char, string> DataTypeEntityKeyKeyword = Tok("entity_keys");

    private static readonly Parser<char, string> OperationGetKeyword = Tok("Get");
    private static readonly Parser<char, string> OperationPostKeyword = Tok("post");
    private static readonly Parser<char, string> OperationPutKeyword = Tok("put");
    private static readonly Parser<char, string> OperationDeleteKeyword = Tok("delete");

    private static Parser<char, T> Parenthesised<T>(Parser<char, T> parser)
        => parser.Between(BracketLeft, BracketRight);

    private static readonly Parser<char, NameType> NameIdentifier =
        Tok(from first in Letter
                from rest in OneOf(Letter, Digit, Char('_')).ManyString()
                select NameType.FromString(first + rest))
            .Labelled("Name");

    private static readonly Parser<char, bool> TemporalSyntax =
        Tok("temporal").Select(_ => true);

    private static readonly Parser<char, bool> SoftDeleteSyntax =
        Tok("soft_delete").Select(_ => true);

    private static readonly Parser<char, AttributeType> KeyValueSyntax =
        from en in Tok(OneOf(Letter, Digit, Char('_'))).ManyString()
        from eq in Tok("=").Optional().IgnoreResult()
        from n in Tok(OneOf(Letter, Digit, Char('_'))).ManyString().Optional()
        select new AttributeType(en, n);

    private static readonly Parser<char, DataTypeDetail> DataTypeSyntax =
        from name in OneOf(DataTypeBoolKeyword, DataTypeStringKeyword, DataTypeDateTimeKeyword, DataTypeDateKeyword,
            DataTypeTimeKeyword, DataTypeDateTimeOffsetKeyword, DataTypeByteKeyword, DataTypeEnumKeyword, DataTypeCurrencyKeyword, DataTypeSingleEntityKeyword,
            DataTypeMultipleEntityKeyword, DataTypeLongKeyword, DataTypeULongKeyword, DataTypeDocumentKeyword,
            DataTypeIntKeyword, DataTypeImageKeyword, DataTypeDoubleKeyword, DataTypeLatLongKeyword, DataTypePercentageKeyword)
        from attrs in KeyValueSyntax.Separated(Comma).Between(BracketLeft, BracketRight)
        select new DataTypeDetail(name, attrs);

    private static readonly Parser<char, DataTypeDetail> DataTypeRpcSyntax =
        from name in OneOf(DataTypeBoolKeyword, DataTypeStringKeyword, DataTypeDateTimeKeyword, DataTypeDateKeyword,
            DataTypeTimeKeyword, DataTypeDateTimeOffsetKeyword, DataTypeByteKeyword, DataTypeEnumKeyword, DataTypeCurrencyKeyword, DataTypeSingleEntityKeyword,
            DataTypeMultipleEntityKeyword, DataTypeLongKeyword, DataTypeULongKeyword, DataTypeDocumentKeyword, DataTypeRpcTypeKeyword, DataTypeEntityKeyKeyword,
            DataTypeIntKeyword, DataTypeImageKeyword, DataTypeDoubleKeyword, DataTypeLatLongKeyword, DataTypePercentageKeyword)
        from attrs in KeyValueSyntax.Separated(Comma).Between(BracketLeft, BracketRight).Optional()
        select new DataTypeDetail(name, attrs.HasValue ? attrs.Value : []);

    private static readonly Parser<char, string> VersionSyntax =
        Try(
            LetterOrDigit.ManyString().Before(Period).Select(s => s)
        ).Labelled("Version");

    internal static readonly Parser<char, VersionedName> EntitySyntax =
        from kw in ObjectEntityKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> RpcTypeSyntax =
        from kw in ObjectRpcTypeKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> DomainSyntax =
        from kw in ObjectDomainKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> EndpointSyntax =
        from kw in ObjectEndpointKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> EnumSyntax =
        from kw in ObjectEnumKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);
    
    internal static readonly Parser<char, VersionedName> ServiceSyntax =
        from kw in ObjectServiceKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);
    
    internal static readonly Parser<char, VersionedName> FlagSyntax =
        from kw in ObjectFlagKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    internal static readonly Parser<char, VersionedName> RpcSyntax =
        from kw in ObjectRpcKeyword
        from v in VersionSyntax.Optional()
        from n in NameIdentifier
        select new VersionedName(n, v);

    private static readonly Parser<char, string> ClearText =
        Token(c => c != '"')
            .ManyString()
            .Between(Quote);

    internal static readonly Parser<char, NonEmptyString> DisableApiSyntax = 
        Tok("disableApi")
            .Then(Parenthesised(ClearText.Select(s => new NonEmptyString(s))))
            .Labelled("'disableApi()'");
    
    internal static readonly Parser<char, NonEmptyString> DescriptionSyntax =
        AttributeDescriptionKeyword
            .Then(Parenthesised(ClearText.Select(s => new NonEmptyString(s))))
            .Labelled("'description(\"<clear text description>\")'");

    private static readonly Parser<char, Unit> SkipComment = SkipWhitespaces
        .Then(Try(SkipLineComment(Char('#'))
            .SeparatedAndOptionallyTerminated(SkipWhitespaces)
            .IgnoreResult()))
        .Before(SkipWhitespaces);

    private static readonly Parser<char, ServiceEnums> EnumsSyntax =
        ServiceEnumsKeyword
            .Then(NameIdentifier.SeparatedAndOptionallyTerminatedAtLeastOnce(Comma).Between(BraceLeft, BraceRight))
            .Select(x=>new ServiceEnums(x));

    private static readonly Parser<char, ServiceEntities> EntitiesSyntax =
        ServiceEntitiesKeyword
            .Then(NameIdentifier.SeparatedAndOptionallyTerminatedAtLeastOnce(Comma).Between(BraceLeft, BraceRight))
            .Select(x => new ServiceEntities(x));

    internal static readonly Parser<char,ReferenceNames> ReferenceNameSyntax = 
        from n in NameIdentifier
        from c in Colon
        from i in NameIdentifier
            .SeparatedAndOptionallyTerminatedAtLeastOnce(Comma)
            .Between(BraceLeft, BraceRight)
        select new ReferenceNames(n, i);
    
    internal static readonly Parser<char, ServiceReferences> ReferencesSyntax =
        ServiceReferencesKeyword
            .Then(ReferenceNameSyntax
                .SeparatedAndOptionallyTerminatedAtLeastOnce(Comma)
                .Between(BraceLeft, BraceRight))
            .Select(x => new ServiceReferences(x));

    internal static readonly Parser<char, ServiceImplementsRpcs> ImplementsRpcsSyntax =
        ServiceImplementsRpcsKeyword
            .Then(NameIdentifier.SeparatedAndOptionallyTerminatedAtLeastOnce(Comma).Between(BraceLeft, BraceRight))
            .Select(x => new ServiceImplementsRpcs(x));

    private static readonly Parser<char, ServiceCallsRpcs> CallsRpcsSyntax =
        ServiceCallsRpcsKeyword
            .Then(NameIdentifier.SeparatedAndOptionallyTerminatedAtLeastOnce(Comma).Between(BraceLeft, BraceRight))
            .Select(x => new ServiceCallsRpcs(x));

    private static readonly Parser<char, ServiceContent> ContentSyntax =
        from enums in EnumsSyntax.Optional()
        from c1 in Comma.IgnoreResult().Optional()
        from entities in EntitiesSyntax.Optional()
        from c2 in Comma.IgnoreResult().Optional()
        from references in ReferencesSyntax.Optional()
        from c3 in Comma.IgnoreResult().Optional()
        from callRpcs in CallsRpcsSyntax.Optional()
        from c4 in Comma.IgnoreResult().Optional()
        from impls in ImplementsRpcsSyntax.Optional()
        select new ServiceContent(
            enums.GetValueOrDefault(), 
            entities.GetValueOrDefault(), 
            references.GetValueOrDefault(), 
            callRpcs.GetValueOrDefault(), 
            impls.GetValueOrDefault());

    internal static readonly Parser<char, FieldDetail> FieldSyntax =
        from n in NameIdentifier
        from colon in Colon
        from dt in DataTypeSyntax.Before(Comma)
        from s in DescriptionSyntax
        from c in Comma.IgnoreResult().Optional()
        from t in TemporalSyntax.Optional()
        select new FieldDetail(n, dt, s, t.HasValue);

    internal static readonly Parser<char, FieldDetail> RpcFieldSyntax =
        from n in NameIdentifier
        from colon in Colon
        from dt in DataTypeRpcSyntax.Before(Comma)
        from s in DescriptionSyntax
        from c in Comma.Optional()
        from t in TemporalSyntax.Optional()
        select new FieldDetail(n, dt, s, t.HasValue);
    
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
        from kw in ObjectEntityKeyKeyword
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
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new EntityKeyBuilder(en, k, kt.HasValue, desc, fields);

    private static readonly Parser<char, EntityBuilder> EntityParserRule =
        from c in SkipComment
        from en in EntitySyntax
        from co in Colon
        from desc in DescriptionSyntax
        from comma in Comma.Optional()
        from da in DisableApiSyntax.Optional()
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new EntityBuilder(en, desc, fields);

    private static readonly Parser<char, TypeBuilder> RpcTypeParserRule =
        from c in SkipComment
        from en in RpcTypeSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from fields in RpcFieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new TypeBuilder(en, desc, fields);

    private static readonly Parser<char, DomainBuilder> DomainParserRule =
        from c in SkipComment
        from en in DomainSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from fields in FieldSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new DomainBuilder(en, desc, fields);

    private static readonly Parser<char, OwnerDetail> OwnerSyntax =
        from kw in OwnerKeyword
        from n in Parenthesised(NameIdentifier)
        select new OwnerDetail(n);
    
    private static readonly Parser<char, string> OperationSyntax =
        from kw in OperationKeyword
        from op in Parenthesised(OneOf(OperationGetKeyword, OperationPostKeyword, OperationPutKeyword, OperationDeleteKeyword).Between(Quote)).Labelled("Operation Type")
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
        from body in EndpointBodySyntax.Between(BraceLeft, BraceRight)
        select new EndpointBuilder(ep,o,desc);
    
    private static readonly Parser<char, EnumBuilder> EnumParserRule =
        from c in SkipComment
        from en in EnumSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from values in EnumValueSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new EnumBuilder(en, desc, values);

    private static readonly Parser<char, FlagBuilder> FlagParserRule =
        from c in SkipComment
        from en in FlagSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from values in FlagValueSyntax.SeparatedAndOptionallyTerminated(Whitespaces).Between(BraceLeft, BraceRight)
        select new FlagBuilder(en, desc, values);
    
    private static readonly Parser<char, ServiceBuilder> ServiceParserRule =
        from c in SkipComment
        from en in ServiceSyntax
        from co in Colon
        from desc in DescriptionSyntax
        from content in ContentSyntax.Between(BraceLeft, BraceRight)
        select new ServiceBuilder(en, desc, content);

    private static readonly Parser<char, RpcBuilder> RpcParserRule =
        from c in SkipComment
        from en in RpcSyntax
        from co in Colon
        from desc in DescriptionSyntax
        select new RpcBuilder(en, desc, null, null, 0);

    public static readonly Func<string, Builder> ParseEntity = input => EntityParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseDomain = input => DomainParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseRpcType = input => RpcTypeParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEntityKey= input => EntityKeyParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEnum = input => EnumParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseService = input => ServiceParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseFlag = input => FlagParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseRpc = input => RpcParserRule.ParseOrThrow(input);

    public static readonly Func<string, Builder> ParseEndpoint = input => EndpointParserRule.ParseOrThrow(input);
}
