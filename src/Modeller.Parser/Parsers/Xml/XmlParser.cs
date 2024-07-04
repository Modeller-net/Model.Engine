namespace Modeller.Parsers;

public static class XmlParser
{
    public static Result<char, Tag> Parse(string input) => Node.Parse(input);
    private static readonly Parser<char, string> Identifier = Parser<char>.Token(char.IsLetter).SelectMany(first => Parser<char>.Token(char.IsLetterOrDigit).ManyString(), (first, rest) => first + rest);
    private static readonly Parser<char, char> Lt = Parser.Char('<');
    private static readonly Parser<char, char> Gt = Parser.Char('>');
    private static readonly Parser<char, char> Quote = Parser.Char('"');
    private static readonly Parser<char, char> Equal = Parser.Char('=');
    private static readonly Parser<char, char> Slash = Parser.Char('/');
    private static readonly Parser<char, Unit> SlashGT = Slash.Then(Parser.Whitespaces).Then(Gt).Then(Parser<char>.Return(Unit.Value));
    private static readonly Parser<char, Unit> LtSlash = Lt.Then(Parser.Whitespaces).Then(Slash).Then(Parser<char>.Return(Unit.Value));
    private static readonly Parser<char, string> AttrValue = Parser<char>.Token(c => c != '"').ManyString();
    private static readonly Parser<char, Attribute> Attr =
        from name in Identifier
        from eq in Equal.Between(Parser.SkipWhitespaces)
        from val in AttrValue.Between(Quote)
        select new Attribute(name, val);

    private static readonly Parser<char, OpeningTagInfo> TagBody =
        from name in Identifier
        from attrs in (
            from ws in Parser.Try(Parser.Whitespace.SkipAtLeastOnce())
            from attrs in Attr.Separated(Parser.SkipWhitespaces)
            select attrs
        ).Optional()
        select new OpeningTagInfo(name, attrs.GetValueOrDefault(Enumerable.Empty<Attribute>()));

    private static readonly Parser<char, Tag> EmptyElementTag =
        from opening in Lt
        from body in TagBody.Between(Parser.SkipWhitespaces)
        from closing in SlashGT
        select new Tag(body.Name, body.Attributes, null);

    private static readonly Parser<char, OpeningTagInfo> OpeningTag = TagBody.Between(Parser.SkipWhitespaces).Between(Lt, Gt);
    private static Parser<char, string> ClosingTag => Identifier.Between(Parser.SkipWhitespaces).Between(LtSlash, Gt);
    private static readonly Parser<char, Tag> Tag =
        from open in OpeningTag
        from children in Parser.Try(Node!).Separated(Parser.SkipWhitespaces).Between(Parser.SkipWhitespaces)
        from close in ClosingTag
        where open.Name.Equals(close, StringComparison.Ordinal)
        select new Tag(open.Name, open.Attributes, children);

    private static readonly Parser<char, Tag> Node = Parser.Try(EmptyElementTag).Or(Tag);

    private struct OpeningTagInfo(string name, IEnumerable<Attribute> attributes)
    {
        public string Name { get; } = name;

        public IEnumerable<Attribute> Attributes { get; } = attributes;
    }
}
