namespace Modeller.ParserTests;

public class StringParserTests : ParserTestBase
{
    [Fact]
    public void TestReturn()
    {
        {
            var parser = Parser<char>.Return('a');
            AssertPartialParse(parser, "", 'a', 0);
            AssertPartialParse(parser, "foobar", 'a', 0);
        }

        {
            var parser = Parser<char>.FromResult('a');
            AssertPartialParse(parser, "", 'a', 0);
            AssertPartialParse(parser, "foobar", 'a', 0);
        }
    }

    [Fact]
    public void TestFail()
    {
        {
            var parser = Parser<char>.Fail<Unit>("message");
            var expectedError = new ParseError<char>(
                Maybe.Nothing<char>(),
                false,
                ImmutableArray.Create(new Expected<char>(ImmutableArray.Create<char>())),
                0,
                SourcePosDelta.Zero,
                "message"
            );
            AssertFailure(parser, "", expectedError);
            AssertFailure(parser, "foobar", expectedError);
        }
    }

    [Fact]
    public void TestToken()
    {
        {
            var parser = Parser.Char('a');
            AssertPartialParse(parser, "a", 'a', 1);
            AssertPartialParse(parser, "ab", 'a', 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('a'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "b",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('a'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.AnyCharExcept('a', 'b', 'c');
            AssertPartialParse(parser, "e", 'e', 1);
            AssertFailure(
                parser,
                "b",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Token('a'.Equals);
            AssertPartialParse(parser, "a", 'a', 1);
            AssertPartialParse(parser, "ab", 'a', 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "b",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Any;
            AssertPartialParse(parser, "a", 'a', 1);
            AssertPartialParse(parser, "b", 'b', 1);
            AssertPartialParse(parser, "ab", 'a', 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("any character")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.Whitespace;
            AssertPartialParse(parser, "\r", '\r', 1);
            AssertPartialParse(parser, "\n", '\n', 1);
            AssertPartialParse(parser, "\t", '\t', 1);
            AssertPartialParse(parser, " ", ' ', 1);
            AssertPartialParse(parser, " abc", ' ', 1);
            AssertFailure(
                parser,
                "abc",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(new Expected<char>("whitespace")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("whitespace")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestCIChar()
    {
        {
            var parser = Parser.CiChar('a');
            AssertPartialParse(parser, "a", 'a', 1);
            AssertPartialParse(parser, "ab", 'a', 1);
            AssertPartialParse(parser, "A", 'A', 1);
            AssertPartialParse(parser, "AB", 'A', 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create('A')), new Expected<char>(ImmutableArray.Create('a'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "b",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create('A')), new Expected<char>(ImmutableArray.Create('a'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestEnd()
    {
        {
            var parser = Parser<char>.End;
            AssertPartialParse(parser, "", Unit.Value, 0);
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(default(Expected<char>)),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestNumber()
    {
        {
            var parser = Parser.Num;
            AssertFullParse(parser, "0", 0);
            AssertFullParse(parser, "+0", +0);
            AssertFullParse(parser, "-0", -0);
            AssertFullParse(parser, "1", 1);
            AssertFullParse(parser, "+1", +1);
            AssertFullParse(parser, "-1", -1);
            AssertFullParse(parser, "12345", 12345);
            AssertPartialParse(parser, "1a", 1, 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(new Expected<char>("number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "+",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "-",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser = Parser.HexNum;
            AssertFullParse(parser, "09", 0x09);
            AssertFullParse(parser, "ab", 0xab);
            AssertFullParse(parser, "cd", 0xcd);
            AssertFullParse(parser, "ef", 0xef);
            AssertFullParse(parser, "AB", 0xAB);
            AssertFullParse(parser, "CD", 0xCD);
            AssertFullParse(parser, "EF", 0xEF);
            AssertFailure(
                parser,
                "g",
                new(
                    Maybe.Just('g'),
                    false,
                    ImmutableArray.Create(new Expected<char>("hexadecimal number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.OctalNum;
            AssertFullParse(parser, "7", 7);
            AssertFailure(
                parser,
                "8",
                new(
                    Maybe.Just('8'),
                    false,
                    ImmutableArray.Create(new Expected<char>("octal number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.LongNum;
            AssertFullParse(parser, "0", 0L);
            AssertFullParse(parser, "+0", +0L);
            AssertFullParse(parser, "-0", -0L);
            AssertFullParse(parser, "1", 1L);
            AssertFullParse(parser, "+1", +1L);
            AssertFullParse(parser, "-1", -1L);
            AssertFullParse(parser, "12345", 12345L);
            var tooBigForInt = ((long)int.MaxValue) + 1;
            AssertFullParse(parser, tooBigForInt.ToString(null as IFormatProvider), tooBigForInt);
            AssertPartialParse(parser, "1a", 1, 1);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(new Expected<char>("number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "+",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "-",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser = Parser.Real;
            AssertFullParse(parser, "0", 0d);
            AssertFullParse(parser, "+0", +0d);
            AssertFullParse(parser, "-0", -0d);
            AssertFullParse(parser, "1", 1d);
            AssertFullParse(parser, "+1", +1d);
            AssertFullParse(parser, "-1", -1d);

            AssertFullParse(parser, "12345", 12345d);
            AssertFullParse(parser, "+12345", +12345d);
            AssertFullParse(parser, "-12345", -12345d);

            AssertFullParse(parser, "12.345", 12.345d);
            AssertFullParse(parser, "+12.345", +12.345d);
            AssertFullParse(parser, "-12.345", -12.345d);

            AssertFullParse(parser, ".12345", .12345d);
            AssertFullParse(parser, "+.12345", +.12345d);
            AssertFullParse(parser, "-.12345", -.12345d);

            AssertFullParse(parser, "12345e10", 12345e10d);
            AssertFullParse(parser, "+12345e10", +12345e10d);
            AssertFullParse(parser, "-12345e10", -12345e10d);
            AssertFullParse(parser, "12345e+10", 12345e+10d);
            AssertFullParse(parser, "+12345e+10", +12345e+10d);
            AssertFullParse(parser, "-12345e+10", -12345e+10d);
            AssertFullParse(parser, "12345e-10", 12345e-10d);
            AssertFullParse(parser, "+12345e-10", +12345e-10d);
            AssertFullParse(parser, "-12345e-10", -12345e-10d);

            AssertFullParse(parser, "12.345e10", 12.345e10d);
            AssertFullParse(parser, "+12.345e10", +12.345e10d);
            AssertFullParse(parser, "-12.345e10", -12.345e10d);
            AssertFullParse(parser, "12.345e+10", 12.345e+10d);
            AssertFullParse(parser, "+12.345e+10", +12.345e+10d);
            AssertFullParse(parser, "-12.345e+10", -12.345e+10d);
            AssertFullParse(parser, "12.345e-10", 12.345e-10d);
            AssertFullParse(parser, "+12.345e-10", +12.345e-10d);
            AssertFullParse(parser, "-12.345e-10", -12.345e-10d);

            AssertFullParse(parser, ".12345e10", .12345e10d);
            AssertFullParse(parser, "+.12345e10", +.12345e10d);
            AssertFullParse(parser, "-.12345e10", -.12345e10d);
            AssertFullParse(parser, ".12345e+10", .12345e+10d);
            AssertFullParse(parser, "+.12345e+10", +.12345e+10d);
            AssertFullParse(parser, "-.12345e+10", -.12345e+10d);
            AssertFullParse(parser, ".12345e-10", .12345e-10d);
            AssertFullParse(parser, "+.12345e-10", +.12345e-10d);
            AssertFullParse(parser, "-.12345e-10", -.12345e-10d);

            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "+",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "-",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "12345.",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    6,
                    new(0, 6),
                    null
                )
            );
            AssertFailure(
                parser,
                "12345e",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    6,
                    new(0, 6),
                    null
                )
            );
            AssertFailure(
                parser,
                "12345e+",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    7,
                    new(0, 7),
                    null
                )
            );
            AssertFailure(
                parser,
                "12345.e",
                new(
                    Maybe.Just('e'),
                    false,
                    ImmutableArray.Create(new Expected<char>("real number")),
                    6,
                    new(0, 6),
                    null
                )
            );
        }
    }

    [Fact]
    [UseCulture("nb-NO")]
    public void TestRealParserWithDifferentCultureInfo()
    {
        var parser = Parser.Real;
        AssertFullParse(parser, "12.345", 12.345d);
    }

    [Fact]
    public void TestSequence()
    {
        {
            var parser = Parser.String("foo");
            AssertFullParse(parser, "foo", "foo");
            AssertPartialParse(parser, "food", "foo", 3);
            AssertFailure(parser, "bar", new(Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
        }

        {
            var parser = Parser<char>.Sequence(Parser.Char('f'), Parser.Char('o'), Parser.Char('o'));
            AssertFullParse(parser, "foo", "foo".ToArray());
            AssertPartialParse(parser, "food", "foo".ToArray(), 3);
            AssertFailure(parser, "bar", new (Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("f"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "foul", new (Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("o"))), 2, new (0, 2), null));
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("f"))), 0, SourcePosDelta.Zero, null));
        }
    }

    [Fact]
    public void TestCIString()
    {
        {
            var parser = Parser.CiString("foo");
            AssertFullParse(parser, "foo", "foo");
            AssertPartialParse(parser, "food", "foo", 3);
            AssertFullParse(parser, "FOO", "FOO");
            AssertPartialParse(parser, "FOOD", "FOO", 3);
            AssertFullParse(parser, "fOo", "fOo");
            AssertPartialParse(parser, "Food", "Foo", 3);
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "FOul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestBind()
    {
        {
            // any two equal characters
            var parser = Parser<char>.Any.Then(c => Parser<char>.Token(c.Equals));
            AssertFullParse(parser, "aa", 'a');
            AssertFailure(
                parser,
                "ab",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Any.Bind(c => Parser<char>.Token(c.Equals), (x, y) => new { x, y });
            AssertFullParse(parser, "aa", new { x = 'a', y = 'a' });
            AssertFailure(
                parser,
                "ab",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Any.Then(c => Parser<char>.Token(c.Equals), (x, y) => new { x, y });
            AssertFullParse(parser, "aa", new { x = 'a', y = 'a' });
            AssertFailure(
                parser,
                "ab",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser =
                from x in Parser<char>.Any
                from y in Parser<char>.Token(x.Equals)
                select new { x, y };
            AssertFullParse(parser, "aa", new { x = 'a', y = 'a' });
            AssertFailure(
                parser,
                "ab",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }

        {
            var parser = Parser.Char('x').Then(c => Parser.Char('y'));
            AssertFullParse(parser, "xy", 'y');
            AssertFailure(
                parser,
                "yy",
                new(
                    Maybe.Just('y'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('x'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "xx",
                new(
                    Maybe.Just('x'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('y'))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestThen()
    {
        {
            var parser = Parser.Char('a').Then(Parser.Char('b'));
            AssertFullParse(parser, "ab", 'b');
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("b"))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("a"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.Char('a').Then(Parser.Char('b'), (a, b) => new { a, b });
            AssertFullParse(parser, "ab", new { a = 'a', b = 'b' });
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("b"))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("a"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.Char('a').Before(Parser.Char('b'));
            AssertFullParse(parser, "ab", 'a');
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("b"))),
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("a"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestMap()
    {
        {
            var parser = Parser.Map((x, y, z) => new { x, y, z }, Parser.Char('a'), Parser.Char('b'), Parser.Char('c'));
            AssertFullParse(parser, "abc", new { x = 'a', y = 'b', z = 'c' });
            AssertFailure(
                parser,
                "abd",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("c"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            var parser = Parser.Char('a').Select(a => new { a });
            AssertFullParse(parser, "a", new { a = 'a' });
        }

        {
            var parser = Parser.Char('a').Map(a => new { a });
            AssertFullParse(parser, "a", new { a = 'a' });
        }

        {
            var parser =
                from a in Parser.Char('a')
                select new { a };
            AssertFullParse(parser, "a", new { a = 'a' });
        }
    }

    [Fact]
    public void TestOr()
    {
        {
            var parser = Parser<char>.Fail<char>("test").Or(Parser.Char('a'));
            AssertFullParse(parser, "a", 'a');
            AssertFailure(
                parser,
                "b",
                new(
                    Maybe.Nothing<char>(),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create<char>()), new Expected<char>(ImmutableArray.Create('a'))),
                    0,
                    SourcePosDelta.Zero,
                    "test"
                )
            );
        }

        {
            var parser = Parser.Char('a').Or(Parser.Char('b'));
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, "b", 'b');
            AssertFailure(
                parser,
                "c",
                new(
                    Maybe.Just('c'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create('a')), new Expected<char>(ImmutableArray.Create('b'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.String("foo").Or(Parser.String("bar"));
            AssertFullParse(parser, "foo", "foo");
            AssertFullParse(parser, "bar", "bar");
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            var parser = Parser.String("foo").Or(Parser.String("foul"));

            // because the first parser consumed input
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            var parser = Parser.Try(Parser.String("foo")).Or(Parser.String("foul"));
            AssertFullParse(parser, "foul", "foul");
        }
    }

    [Fact]
    public void TestOneOf()
    {
        {
            var parser = Parser.OneOf(Parser.Char('a'), Parser.Char('b'), Parser.Char('c'));
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, "b", 'b');
            AssertFullParse(parser, "c", 'c');
            AssertFailure(
                parser,
                "d",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create('a')), new(ImmutableArray.Create('b')), new Expected<char>(ImmutableArray.Create('c'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.OneOf("abc");
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, "b", 'b');
            AssertFullParse(parser, "c", 'c');
            AssertFailure(
                parser,
                "d",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create('a')), new(ImmutableArray.Create('b')), new Expected<char>(ImmutableArray.Create('c'))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.OneOf(Parser.String("foo"), Parser.String("bar"));
            AssertFullParse(parser, "foo", "foo");
            AssertFullParse(parser, "bar", "bar");
            AssertFailure(
                parser,
                "quux",
                new(
                    Maybe.Just('q'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.CreateRange("foo")), new Expected<char>(ImmutableArray.CreateRange("bar"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestCIOneOf()
    {
        {
            var parser = Parser.CIOneOf('a', 'b', 'c');
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, "b", 'b');
            AssertFullParse(parser, "c", 'c');
            AssertFullParse(parser, "A", 'A');
            AssertFullParse(parser, "B", 'B');
            AssertFullParse(parser, "C", 'C');
            AssertFailure(
                parser,
                "d",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(
                        new Expected<char>(ImmutableArray.Create('a')),
                        new Expected<char>(ImmutableArray.Create('A')),
                        new Expected<char>(ImmutableArray.Create('b')),
                        new Expected<char>(ImmutableArray.Create('B')),
                        new Expected<char>(ImmutableArray.Create('c')),
                        new Expected<char>(ImmutableArray.Create('C'))
                    ),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.CIOneOf("abc");
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, "b", 'b');
            AssertFullParse(parser, "c", 'c');
            AssertFullParse(parser, "A", 'A');
            AssertFullParse(parser, "B", 'B');
            AssertFullParse(parser, "C", 'C');
            AssertFailure(
                parser,
                "d",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(
                        new Expected<char>(ImmutableArray.Create('a')),
                        new Expected<char>(ImmutableArray.Create('A')),
                        new Expected<char>(ImmutableArray.Create('b')),
                        new Expected<char>(ImmutableArray.Create('B')),
                        new Expected<char>(ImmutableArray.Create('c')),
                        new Expected<char>(ImmutableArray.Create('C'))
                    ),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestNot()
    {
        {
            var parser = Parser.Not(Parser.String("food")).Then(Parser.String("bar"));
            AssertFullParse(parser, "foobar", "bar");
        }

        {
            var parser = Parser.Not(Parser.OneOf(Parser.Char('a'), Parser.Char('b'), Parser.Char('c')));
            AssertPartialParse(parser, "e", Unit.Value, 0);
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            var parser = Parser.Not(Parser<char>.Return('f'));
            AssertFailure(
                parser,
                "foobar",
                new(
                    Maybe.Just('f'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            // test to make sure it doesn't throw out the buffer, for the purposes of computing error position
            var str = new string('a', 10000);
            var parser = Parser.Not(Parser.String(str));
            AssertFailure(
                parser,
                str,
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }

        {
            // test error pos calculation
            var parser = Parser.Char('a').Then(Parser.Not(Parser.Char('b')));
            AssertFailure(
                parser,
                "ab",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray<Expected<char>>.Empty,
                    1,
                    SourcePosDelta.OneCol,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestLookahead()
    {
        {
            var parser = Parser.Lookahead(Parser.String("foo"));
            AssertPartialParse(parser, "foo", "foo", 0);
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foe",
                new(
                    Maybe.Just('e'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            // should backtrack on success
            var parser = Parser.Lookahead(Parser.String("foo")).Then(Parser.String("food"));
            AssertFullParse(parser, "food", "food");
        }
    }

    [Fact]
    public void TestRecoverWith()
    {
        {
            var parser = Parser.String("foo").ThenReturn((ParseError<char>?)null)
                .RecoverWith(err => Parser.String("bar").ThenReturn(err)!);

            AssertFullParse(
                parser,
                "fobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            var parser = Parser.String("nabble").ThenReturn((ParseError<char>?)null)
                .Or(
                    Parser.String("foo").ThenReturn((ParseError<char>?)null)
                        .RecoverWith(err => Parser.String("bar").ThenReturn(err)!)
                );

            // shouldn't get the expected from nabble
            AssertFullParse(
                parser,
                "fobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestTryUsingStaticExample()
    {
        {
            static string MkString(char first, IEnumerable<char> rest)
            {
                var sb = new StringBuilder();
                sb.Append(first);
                sb.Append(string.Concat(rest));
                return sb.ToString();
            }

            var pUsing = Parser.String("using");
            var pStatic = Parser.String("static");
            var identifier = Parser<char>.Token(char.IsLetter)
                .Then(Parser<char>.Token(char.IsLetterOrDigit).Many(), MkString)
                .Labelled("identifier");
            var usingStatic =
                from kws in Parser.Try(
                    from u in pUsing.Before(Parser.Whitespace.AtLeastOnce())
                    from s in pStatic.Before(Parser.Whitespace.AtLeastOnce())
                    select new { }
                )
                from id in identifier
                select new { isStatic = true, id };
            var notStatic =
                from u in pUsing
                from ws in Parser.Whitespace.AtLeastOnce()
                from id in identifier
                select new { isStatic = false, id };
            var parser = usingStatic.Or(notStatic);

            AssertFullParse(parser, "using static Console", new { isStatic = true, id = "Console" });
            AssertFullParse(parser, "using System", new { isStatic = false, id = "System" });
            AssertFailure(
                parser,
                "usine",
                new(
                    Maybe.Just('e'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("using"))),
                    4,
                    new(0, 4),
                    null
                )
            );
            AssertFailure(
                parser,
                "using 123",
                new(
                    Maybe.Just('1'),
                    false,
                    ImmutableArray.Create(new Expected<char>("identifier")),
                    6,
                    new(0, 6),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestAssert()
    {
        {
            var parser = Parser.Char('a').Assert('a'.Equals);
            AssertFullParse(parser, "a", 'a');
        }

        {
            var parser = Parser.Char('a').Assert('b'.Equals);
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Nothing<char>(),
                    false,
                    ImmutableArray.Create(new Expected<char>("result satisfying assertion")),
                    1,
                    SourcePosDelta.OneCol,
                    "Assertion failed"
                )
            );
        }

        {
            var parser = Parser.Char('a').Where('a'.Equals);
            AssertFullParse(parser, "a", 'a');
        }

        {
            var parser = Parser.Char('a').Where('b'.Equals);
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Nothing<char>(),
                    false,
                    ImmutableArray.Create(new Expected<char>("result satisfying assertion")),
                    1,
                    SourcePosDelta.OneCol,
                    "Assertion failed"
                )
            );
        }
    }

	private static readonly string[] expected = new[] { "foo" };
	private static readonly string[] expectedArray = new[] { "foo", "foo" };
	private static readonly char[] expectedArray1 = new[] { ' ', ' ', ' ', ' ' };
	private static readonly char[] expectedArray2 = new[] { '\r', '\n' };
	private static readonly char[] expectedArray3 = new[] { ' ' };
	private static readonly string[] expectedArray0 = new[] { "foo", "foo", "foo" };

	[Fact]
    public void TestMany()
    {
        {
            var parser = Parser.String("foo").Many();
            AssertPartialParse(parser, "", Enumerable.Empty<string>(), 0);
            AssertPartialParse(parser, "bar", Enumerable.Empty<string>(), 0);
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foofoo", expectedArray);
            AssertPartialParse(parser, "food", expected, 3);
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser.Whitespaces;
            AssertFullParse(parser, "    ", expectedArray1);
            AssertFullParse(parser, "\r\n", expectedArray2);
            AssertPartialParse(parser, " abc", expectedArray3, 1);
            AssertPartialParse(parser, "abc", Enumerable.Empty<char>(), 0);
            AssertPartialParse(parser, "", Enumerable.Empty<char>(), 0);
        }

        {
            var parser = Parser<char>.Return(1).Many();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestManyString()
    {
        {
            var parser = Parser.Char('f').ManyString();
            AssertPartialParse(parser, "", "", 0);
            AssertPartialParse(parser, "bar", "", 0);
            AssertFullParse(parser, "f", "f");
            AssertFullParse(parser, "ff", "ff");
            AssertPartialParse(parser, "fo", "f", 1);
        }

        {
            var parser = Parser.String("f").ManyString();
            AssertPartialParse(parser, "", "", 0);
            AssertPartialParse(parser, "bar", "", 0);
            AssertFullParse(parser, "f", "f");
            AssertFullParse(parser, "ff", "ff");
            AssertPartialParse(parser, "fo", "f", 1);
        }

        {
            var parser = Parser<char>.Return('f').ManyString();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipMany()
    {
        {
            var parser = Parser.String("foo").SkipMany();
            AssertPartialParse(parser, "", Unit.Value, 0);
            AssertPartialParse(parser, "bar", Unit.Value, 0);
            AssertFullParse(parser, "foo", Unit.Value);
            AssertFullParse(parser, "foofoo", Unit.Value);
            AssertPartialParse(parser, "food", Unit.Value, 3);
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser.SkipWhitespaces.Then(Parser.Char('a'));
            AssertFullParse(parser, "    a", 'a');
            AssertFullParse(parser, " \r\n\ta", 'a');
            AssertFullParse(parser, "a", 'a');
            AssertFullParse(parser, new string(' ', 31) + "a", 'a');
            AssertFullParse(parser, new string(' ', 32) + "a", 'a');
            AssertFullParse(parser, new string(' ', 33) + "a", 'a');
            AssertFullParse(parser, new string(' ', 63) + "a", 'a');
            AssertFullParse(parser, new string(' ', 64) + "a", 'a');
            AssertFullParse(parser, new string(' ', 65) + "a", 'a');
        }

        {
            var parser = Parser<char>.Return(1).SkipMany();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestAtLeastOnce()
    {
        {
            var parser = Parser.String("foo").AtLeastOnce();
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "bar", new(Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foofoo", expectedArray);
            AssertPartialParse(parser, "food", expected, 3);
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser<char>.Return(1).AtLeastOnce();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestAtLeastOnceString()
    {
        {
            var parser = Parser.Char('f').AtLeastOnceString();
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('f'))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "b", new(Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('f'))), 0, SourcePosDelta.Zero, null));
            AssertFullParse(parser, "f", "f");
            AssertFullParse(parser, "ff", "ff");
            AssertPartialParse(parser, "fg", "f", 1);
        }

        {
            var parser = Parser.String("f").AtLeastOnceString();
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('f'))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "b", new(Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.Create('f'))), 0, SourcePosDelta.Zero, null));
            AssertFullParse(parser, "f", "f");
            AssertFullParse(parser, "ff", "ff");
            AssertPartialParse(parser, "fg", "f", 1);
        }

        {
            var parser = Parser<char>.Return('f').AtLeastOnceString();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipAtLeastOnce()
    {
        {
            var parser = Parser.String("foo").SkipAtLeastOnce();
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "bar", new(Maybe.Just('b'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFullParse(parser, "foo", Unit.Value);
            AssertFullParse(parser, "foofoo", Unit.Value);
            AssertPartialParse(parser, "food", Unit.Value, 3);
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser<char>.Return(1).SkipAtLeastOnce();
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestUntil()
    {
        {
            var parser = Parser.String("foo").Until(Parser.Char(' '));
            AssertFullParse(parser, " ", Enumerable.Empty<string>());
            AssertPartialParse(parser, " bar", Enumerable.Empty<string>(), 1);
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foofoo ", expectedArray);
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "foo", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 3, new(0, 3), null));
            AssertFailure(parser, "bar", new(Maybe.Just('b'), false, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "food", new(Maybe.Just('d'), false, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 3, new(0, 3), null));
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser<char>.Return(1).Until(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestManyThen()
    {
        {
            var parser = Parser.String("foo").ManyThen(Parser.Char(' '));
            AssertFullParse(parser, " ", (Enumerable.Empty<string>(), ' '));
            AssertPartialParse(parser, " bar", (Enumerable.Empty<string>(), ' '), 1);
            AssertFullParse(parser, "foo ", (expected, ' '));
            AssertFullParse(parser, "foofoo ", (expectedArray, ' '));
            AssertFailure(parser, "", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "foo", new(Maybe.Nothing<char>(), true, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 3, new(0, 3), null));
            AssertFailure(parser, "bar", new(Maybe.Just('b'), false, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 0, SourcePosDelta.Zero, null));
            AssertFailure(parser, "food", new(Maybe.Just('d'), false, ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))), 3, new(0, 3), null));
            AssertFailure(parser, "foul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 2, new(0, 2), null));
            AssertFailure(parser, "foofoul", new(Maybe.Just('u'), false, ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))), 5, new(0, 5), null));
        }

        {
            var parser = Parser<char>.Return(1).ManyThen(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipUntil()
    {
        {
            var parser = Parser.String("foo").SkipUntil(Parser.Char(' '));
            AssertFullParse(parser, " ", Unit.Value);
            AssertPartialParse(parser, " bar", Unit.Value, 1);
            AssertFullParse(parser, "foo ", Unit.Value);
            AssertFullParse(parser, "foofoo ", Unit.Value);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).SkipUntil(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipManyThen()
    {
        {
            var parser = Parser.String("foo").SkipManyThen(Parser.Char(' '));
            AssertFullParse(parser, " ", ' ');
            AssertPartialParse(parser, " bar", ' ', 1);
            AssertFullParse(parser, "foo ", ' ');
            AssertFullParse(parser, "foofoo ", ' ');
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).SkipManyThen(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestAtLeastOnceUntil()
    {
        {
            var parser = Parser.String("foo").AtLeastOnceUntil(Parser.Char(' '));
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foofoo ", expectedArray);
            AssertFailure(
                parser,
                " ",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                " bar",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).AtLeastOnceUntil(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestAtLeastOnceThen()
    {
        {
            var parser = Parser.String("foo").AtLeastOnceThen(Parser.Char(' '));
            AssertFullParse(parser, "foo ", (expected, ' '));
            AssertFullParse(parser, "foofoo ", (expectedArray, ' '));
            AssertFailure(
                parser,
                " ",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                " bar",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).AtLeastOnceThen(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipAtLeastOnceUntil()
    {
        {
            var parser = Parser.String("foo").SkipAtLeastOnceUntil(Parser.Char(' '));
            AssertFullParse(parser, "foo ", Unit.Value);
            AssertFullParse(parser, "foofoo ", Unit.Value);
            AssertFailure(
                parser,
                " ",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                " bar",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).SkipAtLeastOnceUntil(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestSkipAtLeastOnceThen()
    {
        {
            var parser = Parser.String("foo").SkipAtLeastOnceThen(Parser.Char(' '));
            AssertFullParse(parser, "foo ", ' ');
            AssertFullParse(parser, "foofoo ", ' ');
            AssertFailure(
                parser,
                " ",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                " bar",
                new(
                    Maybe.Just(' '),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "food",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new(ImmutableArray.Create(' ')), new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foofoul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    5,
                    new(0, 5),
                    null
                )
            );
        }

        {
            var parser = Parser<char>.Return(1).SkipAtLeastOnceThen(Parser.Char(' '));
            Assert.Throws<InvalidOperationException>(() => parser.Parse(""));
        }
    }

    [Fact]
    public void TestRepeat()
    {
        {
            var parser = Parser.String("foo").Repeat(3);
            AssertFullParse(parser, "foofoofoo", expectedArray0);
            AssertFailure(
                parser,
                "foofoo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    6,
                    new(0, 6),
                    null
                )
            );
        }

        {
            var parser = Parser.Char('f').RepeatString(3);
            AssertFullParse(parser, "fff", "fff");
            AssertFailure(
                parser,
                "ff",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("f"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparated()
    {
        {
            var parser = Parser.String("foo").Separated(Parser.Char(' '));
            AssertPartialParse(parser, "", Enumerable.Empty<string>(), 0);
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foo foo", expectedArray);
            AssertPartialParse(parser, "foobar", expected, 3);
            AssertPartialParse(parser, "bar", Enumerable.Empty<string>(), 0);
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foo bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    4,
                    new(0, 4),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparatedAtLeastOnce()
    {
        {
            var parser = Parser.String("foo").SeparatedAtLeastOnce(Parser.Char(' '));
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foo foo", expectedArray);
            AssertPartialParse(parser, "foobar", expected, 3);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foo bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    4,
                    new(0, 4),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparatedAndTerminated()
    {
        {
            var parser = Parser.String("foo").SeparatedAndTerminated(Parser.Char(' '));
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foo foo ", expectedArray);
            AssertPartialParse(parser, "foo bar", expected, 4);
            AssertPartialParse(parser, "", Array.Empty<string>(), 0);
            AssertPartialParse(parser, "bar", Array.Empty<string>(), 0);
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange(" "))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange(" "))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foo foobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange(" "))),
                    7,
                    new(0, 7),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparatedAndTerminatedAtLeastOnce()
    {
        {
            var parser = Parser.String("foo").SeparatedAndTerminatedAtLeastOnce(Parser.Char(' '));
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foo foo ", expectedArray);
            AssertPartialParse(parser, "foo bar", expected, 4);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "foo",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.Create(' '))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange(" "))),
                    3,
                    new(0, 3),
                    null
                )
            );
            AssertFailure(
                parser,
                "foo foobar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange(" "))),
                    7,
                    new(0, 7),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparatedAndOptionallyTerminated()
    {
        {
            var parser = Parser.String("foo").SeparatedAndOptionallyTerminated(Parser.Char(' '));
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foo foo ", expectedArray);
            AssertFullParse(parser, "foo foo", expectedArray);
            AssertPartialParse(parser, "foo foobar", expectedArray, 7);
            AssertPartialParse(parser, "foo foo bar", expectedArray, 8);
            AssertPartialParse(parser, "foo bar", expected, 4);
            AssertPartialParse(parser, "foobar", expected, 3);
            AssertPartialParse(parser, "", Array.Empty<string>(), 0);
            AssertPartialParse(parser, "bar", Array.Empty<string>(), 0);
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
            AssertFailure(
                parser,
                "foo four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    6,
                    new(0, 6),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestSeparatedAndOptionallyTerminatedAtLeastOnce()
    {
        {
            var parser = Parser.String("foo").SeparatedAndOptionallyTerminatedAtLeastOnce(Parser.Char(' '));
            AssertFullParse(parser, "foo ", expected);
            AssertFullParse(parser, "foo", expected);
            AssertFullParse(parser, "foo foo ", expectedArray);
            AssertFullParse(parser, "foo foo", expectedArray);
            AssertPartialParse(parser, "foo foobar", expectedArray, 7);
            AssertPartialParse(parser, "foo foo bar", expectedArray, 8);
            AssertPartialParse(parser, "foo bar", expected, 4);
            AssertPartialParse(parser, "foobar", expected, 3);
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    true,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "bar",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestBetween()
    {
        {
            var parser = Parser.String("foo").Between(Parser.Char('{'), Parser.Char('}'));
            AssertFullParse(parser, "{foo}", "foo");
        }
    }

    [Fact]
    public void TestOptional()
    {
        {
            var parser = Parser.String("foo").Optional();
            AssertFullParse(parser, "foo", Maybe.Just("foo"));
            AssertPartialParse(parser, "food", Maybe.Just("foo"), 3);
            AssertPartialParse(parser, "bar", Maybe.Nothing<string>(), 0);
            AssertPartialParse(parser, "", Maybe.Nothing<string>(), 0);
            AssertFailure(
                parser,
                "four",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("foo"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }

        {
            var parser = Parser.Try(Parser.String("foo")).Optional();
            AssertFullParse(parser, "foo", Maybe.Just("foo"));
            AssertPartialParse(parser, "food", Maybe.Just("foo"), 3);
            AssertPartialParse(parser, "bar", Maybe.Nothing<string>(), 0);
            AssertPartialParse(parser, "", Maybe.Nothing<string>(), 0);
            AssertPartialParse(parser, "four", Maybe.Nothing<string>(), 0);
        }

        {
            var parser = Parser.Char('+').Optional().Then(Parser.Digit).Select(char.GetNumericValue);
            AssertFullParse(parser, "1", 1);
            AssertFullParse(parser, "+1", 1);
            AssertFailure(
                parser,
                "a",
                new(
                    Maybe.Just('a'),
                    false,
                    ImmutableArray.Create(new Expected<char>("digit")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
        }
    }

    [Fact]
    public void TestMapWithInput()
    {
        {
            var parser = Parser.String("abc").Many().MapWithInput((input, result) => (input.ToString(), result.Count()));
            AssertFullParse(parser, "abc", ("abc", 1));
            AssertFullParse(parser, "abcabc", ("abcabc", 2));

            // long input, to check that it doesn't discard the buffer
            AssertFullParse(
                parser,
                string.Concat(Enumerable.Repeat("abc", 5000)),
                (string.Concat(Enumerable.Repeat("abc", 5000)), 5000)
            );

            AssertFailure(
                parser,
                "abd",
                new(
                    Maybe.Just('d'),
                    false,
                    ImmutableArray.Create(new Expected<char>(ImmutableArray.CreateRange("abc"))),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    [Fact]
    public void TestRec()
    {
        // roughly equivalent to Parser.String("foo").Separated(Parser.Char(' '))
        Parser<char, string>? p2 = null;
        var p1 = Parser.String("foo").Then(
            Parser.Rec(() => p2!).Optional(),
            (x, y) => y.HasValue ? x + y.Value : x
        );
        p2 = Parser.Char(' ').Then(Parser.Rec(() => p1));

        AssertFullParse(p1, "foo foo", "foofoo");
    }

    [Fact]
    public void TestLabelled()
    {
        {
            var p = Parser.String("foo").Labelled("bar");
            AssertFailure(
                p,
                "baz",
                new(
                    Maybe.Just('b'),
                    false,
                    ImmutableArray.Create(new Expected<char>("bar")),
                    0,
                    SourcePosDelta.Zero,
                    null
                )
            );
            AssertFailure(
                p,
                "foul",
                new(
                    Maybe.Just('u'),
                    false,
                    ImmutableArray.Create(new Expected<char>("bar")),
                    2,
                    new(0, 2),
                    null
                )
            );
        }
    }

    private class TestCast1
    {
    }

    private sealed class TestCast2 : TestCast1
    {
        public override bool Equals(object? other) => other is TestCast2;

        public override int GetHashCode() => 1;
    }

    [Fact]
    public void TestCast()
    {
        {
            var parser = Parser<char>.Return(new TestCast2()).Cast<TestCast1>();
            AssertPartialParse(parser, "", new TestCast2(), 0);
        }

        {
            var parser = Parser<char>.Return(new TestCast1()).OfType<TestCast2>();
            AssertFailure(
                parser,
                "",
                new(
                    Maybe.Nothing<char>(),
                    false,
                    ImmutableArray.Create(new Expected<char>("result of type TestCast2")),
                    0,
                    SourcePosDelta.Zero,
                    "Expected a TestCast2 but got a TestCast1"
                )
            );
        }
    }

    [Fact]
    public void TestCurrentPos()
    {
        {
            var parser = Parser<char>.CurrentSourcePosDelta;
            AssertPartialParse(parser, "", SourcePosDelta.Zero, 0);
        }

        {
            var parser = Parser.String("foo").Then(Parser<char>.CurrentSourcePosDelta);
            AssertFullParse(parser, "foo", new(0, 3));
        }

        {
            var parser = Parser.Try(Parser.String("foo")).Or(Parser<char>.Return("")).Then(Parser<char>.CurrentSourcePosDelta);
            AssertPartialParse(parser, "f", SourcePosDelta.Zero, 0);  // it should backtrack
        }
    }

    internal enum TestEnum
    {
        Value1 = 0,
        Value2,
        Value3
    }

    [Fact]
    public void EnumParseTest()
    {
        const string value1 = "value1";
        var result = Parser.Enum<TestEnum>().Parse(value1);
        Assert.False(result.Success);

        result = Parser.CiEnum<TestEnum>().Parse(value1);
        Assert.Equal(TestEnum.Value1, result.Value);

        const string value2 = "Value2";
        result = Parser.Enum<TestEnum>().Parse(value2);
        Assert.Equal(TestEnum.Value2, result.Value);

        result = Parser.CiEnum<TestEnum>().Parse(value2);
        Assert.Equal(TestEnum.Value2, result.Value);
    }
}
