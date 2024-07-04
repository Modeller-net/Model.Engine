namespace Names;

public readonly record struct NonEmptyString(string Value)
{
    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value) ? Value.Trim()
            : throw new ArgumentException("Value cannot be null or whitespace.", nameof(Value));
    
    public static implicit operator string(NonEmptyString value) => value.Value;
    
    public static explicit operator NonEmptyString(string value) => new(value);
    
    public override string ToString() => Value;
}
