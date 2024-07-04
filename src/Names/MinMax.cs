using ErrorOr;

namespace Names;

public abstract record MinMaxType;

internal sealed record NotSet() : MinMaxType;

internal sealed record Min(int Value) : MinMaxType
{
    public int Value { get; } =
        Value > 0 ? Value: throw new ArgumentOutOfRangeException(nameof(Value), "Value must be greater than 0");
    
    public override string ToString() => Value.ToString();
}

internal sealed record Max(int Value) : MinMaxType
{
    public int Value { get; } =
        Value > 0 ? Value: throw new ArgumentOutOfRangeException(nameof(Value), "Value must be greater than 0");
    public override string ToString() => Value.ToString();
}

internal sealed record Range(int Min, int Max) : MinMaxType
{
    public int Min { get; } =
        Min > 0 ? Min: throw new ArgumentOutOfRangeException(nameof(Min), "Min must be greater than 0");
    public int Max { get; } =
        Max >= Min ? Max: throw new ArgumentOutOfRangeException(nameof(Max), "Max must be greater than Min");

    public override string ToString() => $"{Min}-{Max}";
}

public static class MinMax
{
    public static MinMaxType Set(int? min, int? max)
    {
        if (!min.HasValue && !max.HasValue) return new NotSet();
        if (min.HasValue && max.HasValue) return new Range(min.Value, max.Value);
        if (!max.HasValue) return new Min(min!.Value);
        return new Max(max.Value);
    }
    
    public static ErrorOr<int> Max(this MinMaxType minMax) => 
        minMax switch
        {
            Range range => range.Max,
            Max max => max.Value,
            NotSet => Error.NotFound(),
            _ => Error.NotFound()
        };

    public static ErrorOr<int> Min(this MinMaxType minMax) => 
        minMax switch
        {
            Range range => range.Min,
            Min min => min.Value,
            NotSet => Error.NotFound(),
            _ => Error.NotFound()
        };
}