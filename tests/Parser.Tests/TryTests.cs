namespace Modeller.ParserTests;

public class TryTests : ParserTestBase
{
    [Fact]
    public void TestString() => DoTest((p, x) => p.Parse(x), x => x, x => x);

    [Fact]
    public void TestList() => DoTest((p, x) => p.Parse(x), x => x, x => x.ToCharArray());

    [Fact]
    public void TestReadOnlyList() => DoTest((p, x) => p.ParseReadOnlyList(x), x => x, x => x.ToCharArray());

    [Fact]
    public void TestEnumerator() => DoTest((p, x) => p.Parse(x), x => x, x => x.AsEnumerable());

    [Fact]
    public void TestReader() => DoTest((p, x) => p.Parse(x), x => x, x => new StringReader(x));

    [Fact]
    public void TestStream() => DoTest((p, x) => p.Parse(x), Encoding.ASCII.GetBytes, x => new MemoryStream(Encoding.ASCII.GetBytes(x)));

    [Fact]
    public void TestSpan() => DoTest((p, x) => p.Parse(x.Span), x => x, x => x.AsMemory());

    private static void DoTest<TToken, TInput>(
        Func<Parser<TToken, IEnumerable<TToken>>, TInput, Result<TToken, IEnumerable<TToken>>> parseFunc,
        Func<string, IEnumerable<TToken>> render,
        Func<string, TInput> toInput
    )
        where TToken : IEquatable<TToken>
    {
        {
            var parser =
                Parser.Try(Parser<TToken>.Sequence(render("foo")))
                    .Then(Parser<TToken>.Sequence(render("bar")))
                    .Or(Parser<TToken>.Sequence(render("four")));
            AssertSuccess(parseFunc(parser, toInput("foobar")), render("bar"));
            AssertSuccess(parseFunc(parser, toInput("four")), render("four"));  // it should have consumed the "fo" but then backtracked
            AssertFailure(
                parseFunc(parser, toInput("foo")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("bar")))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foobag")),
                new(
                    Maybe.Just(render("g").Single()),
                    false,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("bar")))),
                    5,
                    new(0, 5),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("f")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("four")))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.CreateRange(render("foo"))), new Expected<TToken>(ImmutableArray.CreateRange(render("four")))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foul")),
                new(
                    Maybe.Just(render("l").Single()),
                    false,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("four")))),
                    3,
                    new(0, 3),
                    null
                )
            );
        }

        {
            var parser = Parser.Try(
                Parser<TToken>.Sequence(render("foo")).Then(
                    Parser.Try(Parser<TToken>.Sequence(render("bar"))).Or(Parser<TToken>.Sequence(render("baz")))
                )
            ).Or(Parser<TToken>.Sequence(render("foobat")));
            AssertSuccess(parseFunc(parser, toInput("foobar")), render("bar"));
            AssertSuccess(parseFunc(parser, toInput("foobaz")), render("baz"));

            // "" -> "foo" -> "fooba[r]" -> "foo" -> "fooba[z]" -> "foo" -> "" -> "foobat"
            AssertSuccess(parseFunc(parser, toInput("foobat")), render("foobat"));
            AssertFailure(
                parseFunc(parser, toInput("fooba")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    5,
                    new(0, 5),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foobag")),
                new(
                    Maybe.Just(render("g").Single()),
                    false,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    5,
                    new(0, 5),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foob")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    4,
                    new(0, 4),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foo")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("f")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("foul")),
                new(
                    Maybe.Just(render("u").Single()),
                    false,
                    ImmutableArray.Create(new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parseFunc(parser, toInput("")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(
                        new(ImmutableArray.CreateRange(render("foo"))),
                        new Expected<TToken>(ImmutableArray.CreateRange(render("foobat")))
                    ),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            // Bug #140 - nested Try()s with the same starting location;
            // second (longer) one tries to rewind
            var parser = Parser.Try(
                Parser.Try(Parser<TToken>.Sequence(render("foo")))
                    .Then(Parser<TToken>.Sequence(render("bar")))
            );

            AssertFailure(
                parseFunc(parser, toInput("fooba")),
                new(
                    Maybe.Nothing<TToken>(),
                    true,
                    ImmutableArray.Create(
                        new Expected<TToken>(ImmutableArray.CreateRange(render("bar")))
                    ),
                    5,
                    new(0, 5),
                    null
                )
            );
        }
    }
}
