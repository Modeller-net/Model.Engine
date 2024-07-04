namespace Modeller.Yaml;

/// <summary>
/// This represents the YAML converter entity for <see cref="NameType"/>.
/// </summary>
public class YamlNameConverter : IYamlTypeConverter
{
    /// <summary>
    /// Gets a value indicating whether the current converter supports converting the specified type.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>Returns <c>True</c>, if the current converter supports; otherwise returns <c>False</c>.</returns>
    public bool Accepts(Type type)
    {
        return type == typeof(NameType);
    }

    /// <summary>
    /// Reads an object's state from a YAML parser.
    /// </summary>
    /// <param name="parser"><see cref="IParser"/> instance.</param>
    /// <param name="type"><see cref="Type"/> to convert.</param>
    /// <returns>Returns the <see cref="NameType"/> instance converted.</returns>
    /// <remarks>On deserializing, all formats in the list are used for conversion.</remarks>
    public object ReadYaml(IParser parser, Type type)
    {
        if (parser.Current?.GetType() == typeof(MappingStart))
        {
            parser.MoveNext();

            var value = string.Empty;
            var maintain = string.Empty;
            do
            {
                var property = parser.Consume<Scalar>().Value;
                switch (property)
                {
                    case "value":
                        value = parser.Consume<Scalar>().Value;
                        break;
                    case "maintain":
                        maintain = parser.Consume<Scalar>().Value;
                        break;
                }
            } while (parser.Current?.GetType() == typeof(Scalar));

            parser.MoveNext();
            return string.IsNullOrWhiteSpace(maintain)
                ? NameType.FromString(value)
                : NameType.FromString(maintain, !value.Equals(maintain, StringComparison.Ordinal) );
        }
        var name = parser.Consume<Scalar>().Value;
        return NameType.FromString(name);
    }

    /// <summary>
    /// Writes the specified object's state to a YAML emitter.
    /// </summary>
    /// <param name="emitter"><see cref="IEmitter"/> instance.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="type"><see cref="Type"/> to convert.</param>
    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value is null)
        {
            return;
        }
        var name = (NameType)value;

        var n = NameType.FromString(name);
        if (n.Value == name)
        {
            emitter.Emit(new Scalar(null, name));
            return;
        }

        emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));
        emitter.Emit(new Scalar(null,"value"));
        emitter.Emit(new Scalar(null, n.Value));
        emitter.Emit(new MappingEnd());
    }
}
