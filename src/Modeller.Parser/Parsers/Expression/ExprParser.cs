using Modeller.Expression;

namespace Modeller.Parsers;

public static class ExprParser
{
    private static Parser<char, T> Tok<T>(Parser<char, T> token)
        => Parser.Try(token).Before(Parser.SkipWhitespaces);

    private static Parser<char, string> Tok(string token)
        => Tok(Parser.String(token));

    private static Parser<char, T> Parenthesised<T>(Parser<char, T> parser)
        => parser.Between(Tok("("), Tok(")"));

    private static Parser<char, Func<Expr, Expr, Expr>> Binary(Parser<char, BinaryOperatorType> op)
        => op.Select<Func<Expr, Expr, Expr>>(type => (l, r) => new BinaryOp(type, l, r));

    private static Parser<char, Func<Expr, Expr>> Unary(Parser<char, UnaryOperatorType> op)
        => op.Select<Func<Expr, Expr>>(type => o => new UnaryOp(type, o));

    private static readonly Parser<char, Func<Expr, Expr, Expr>> Add
        = Binary(Tok("+").ThenReturn(BinaryOperatorType.Add));

    private static readonly Parser<char, Func<Expr, Expr, Expr>> Mul
        = Binary(Tok("*").ThenReturn(BinaryOperatorType.Mul));

    private static readonly Parser<char, Func<Expr, Expr>> Neg
        = Unary(Tok("-").ThenReturn(UnaryOperatorType.Neg));

    private static readonly Parser<char, Func<Expr, Expr>> Complement
        = Unary(Tok("~").ThenReturn(UnaryOperatorType.Complement));
    
    private static readonly Parser<char, Expr> Identifier
        = Tok(Parser.Letter.Then(Parser.LetterOrDigit.ManyString(), (h, t) => h + t))
            .Select<Expr>(name => new Identifier(name))
            .Labelled("identifier");

    private static readonly Parser<char, Expr> Literal
        = Tok(Parser.Num)
            .Select<Expr>(value => new Literal(value))
            .Labelled("integer literal");

    private static Parser<char, Func<Expr, Expr>> Call(Parser<char, Expr> subExpr)
        => Parenthesised(subExpr.Separated(Tok(",")))
            .Select<Func<Expr, Expr>>(args => method => new Call(method, [..args]))
            .Labelled("function call");

    private static readonly Parser<char, Expr> Expr = ExpressionParser.Build<char, Expr>(
        expr => (
            Parser.OneOf(
                Identifier,
                Literal,
                Parenthesised(expr).Labelled("parenthesised expression")
            ),
            new[]
            {
                Operator.PostfixChainable(Call(expr)),
                Operator.Prefix(Neg).And(Operator.Prefix(Complement)),
                Operator.InfixL(Mul),
                Operator.InfixL(Add)
            }
        )
    ).Labelled("expression");

    public static Expr ParseOrThrow(string input)
        => Expr.ParseOrThrow(input);
}
