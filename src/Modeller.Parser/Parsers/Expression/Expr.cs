namespace Modeller.Parsers;

public abstract class Expr : IEquatable<Expr>
{
    public abstract bool Equals(Expr? other);

    public override bool Equals(object? obj) => Equals(obj as Expr);

    public override abstract int GetHashCode();
}

public class Identifier(string name) : Expr
{
    public string Name { get; } = name;

    public override bool Equals(Expr? other)
        => other is Identifier i && Name == i.Name;

    public override int GetHashCode() => Name.GetHashCode(StringComparison.Ordinal);
}

public class Literal(int value) : Expr
{
    public int Value { get; } = value;

    public override bool Equals(Expr? other)
        => other is Literal l && Value == l.Value;

    public override int GetHashCode() => Value;
}

[SuppressMessage("naming", "CA1716:Type conflicts with reserved language keyword", Justification = "Example code")]
public class Call(Expr expr, ImmutableArray<Expr> arguments) : Expr
{
    public Expr Expr { get; } = expr;

    public ImmutableArray<Expr> Arguments { get; } = arguments;

    public override bool Equals(Expr? other)
        => other is Call c
        && Expr.Equals(c.Expr)
        && Arguments.SequenceEqual(c.Arguments);

    public override int GetHashCode() => HashCode.Combine(Expr, Arguments);
}

public enum UnaryOperatorType
{
    Neg,
    Complement
}

public class UnaryOp(UnaryOperatorType type, Expr expr) : Expr
{
    public UnaryOperatorType Type { get; } = type;

    public Expr Expr { get; } = expr;

    public override bool Equals(Expr? other)
        => other is UnaryOp u
        && Type == u.Type
        && Expr.Equals(u.Expr);

    public override int GetHashCode() => HashCode.Combine(Type, Expr);
}

public enum BinaryOperatorType
{
    Add,
    Mul
}

public class BinaryOp(BinaryOperatorType type, Expr left, Expr right) : Expr
{
    public BinaryOperatorType Type { get; } = type;

    public Expr Left { get; } = left;

    public Expr Right { get; } = right;

    public override bool Equals(Expr? other)
        => other is BinaryOp b
        && Type == b.Type
        && Left.Equals(b.Left)
        && Right.Equals(b.Right);

    public override int GetHashCode() => HashCode.Combine(Type, Left, Right);
}
