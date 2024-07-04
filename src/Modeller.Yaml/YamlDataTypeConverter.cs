using System.ComponentModel.Design;

namespace Modeller.Yaml;

/// <summary>
/// This represents the YAML converter entity for <see cref="DataTypeType"/>.
/// </summary>
public class YamlDataTypeConverter : IYamlTypeConverter
{
    /// <summary>
    /// Gets a value indicating whether the current converter supports converting the specified type.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>Returns <c>True</c>, if the current converter supports; otherwise returns <c>False</c>.</returns>
    public bool Accepts(Type type)
    {
        return type.IsAssignableTo(typeof(DataTypeType));
    }

    /// <summary>
    /// Reads an object's state from a YAML parser.
    /// </summary>
    /// <param name="parser"><see cref="IParser"/> instance.</param>
    /// <param name="type"><see cref="Type"/> to convert.</param>
    /// <returns>Returns the <see cref="Name"/> instance converted.</returns>
    /// <remarks>On deserializing, all formats in the list are used for conversion.</remarks>
    public object? ReadYaml(IParser parser, Type type)
    {
        if (parser.Current?.GetType() == typeof(MappingStart))
        {
            parser.MoveNext();

            var parameters = new Dictionary<string, string>();
            do
            {
                var key = parser.Consume<Scalar>().Value;
                var value = parser.Consume<Scalar>().Value;

                parameters.Add(key, value);

            } while (parser.Current?.GetType() == typeof(Scalar));

            parser.MoveNext();

            return BuildDataType(parameters);
        }

        var name = parser.Consume<Scalar>().Value;
        return CreateByType(name);
    }

    private static DataTypeType BuildDataType(Dictionary<string, string> parameters)
    {
        switch (parameters["type"])
        {
            case "string":
                parameters.TryGetValue("unicode", out var unicode);
                parameters.TryGetValue("maxLength", out var maxLength);
                parameters.TryGetValue("minLength", out var minLength);
                int.TryParse(minLength, out var min);
                int.TryParse(maxLength, out var max);
                bool.TryParse(unicode, out var supportUnicode);
                return DataType.String(MinMax.Set(min, max), supportUnicode: supportUnicode);
            case "decimal":
                parameters.TryGetValue("precision", out var precision);
                parameters.TryGetValue("scale", out var scale);
                int.TryParse(precision, out var p);
                int.TryParse(scale, out var s);
                return DataType.Decimal(p, s);
            case "enum" when parameters.TryGetValue("typeName", out var enumTypeName):
                return DataType.Enum(new(enumTypeName));
            case "entity" when parameters.TryGetValue("typeName", out var dataTypeTypeName):
                return DataType.Entity(new(dataTypeTypeName));
            default:
                return CreateByType(parameters["type"]);
        }
    }

    private static DataTypeType CreateByType(string type) => type switch
    {
        "bool" => DataType.Bool(),
        "byte" => DataType.Byte(),
        "Date" => DataType.Date(),
        "DateTime" => DataType.DateTime(),
        "DateTimeOffset" => DataType.DateTimeOffset(),
        "short" => DataType. Int16(),
        "int" => DataType.Int32(),
        "long" => DataType.Int64(),
        "object" => DataType.Object(),
        "time" => DataType.Time(),
        "Guid" => DataType.UniqueIdentifier(),
        _ => DataType.String()
    };

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

        var dt = (DataTypeType)value;

        List<Scalar> scalars = [];
        if (dt.CollectionType != CollectionTypes.None)
        {
            scalars.AddRange(new[]
            {
                new Scalar(null, "As"),
                new Scalar(null, dt.CollectionType.ToString())
            });
        }

        // switch (value)
        // {
        //     case EnumDataType edt:
        //         scalars.AddRange(GetScalars(edt));
        //         break;
        //     case StringDataType sdt:
        //         scalars.AddRange(GetScalars(sdt));
        //         break;
        //     case DecimalDataType ddt:
        //         scalars.AddRange(GetScalars(ddt));
        //         break;
        //     case ObjectDataType odt:
        //         scalars.AddRange(GetScalars(odt));
        //         break;
        // }

        if (scalars.Count == 0)
        {
            emitter.Emit(new Scalar(null, dt.ToString()));
            return;
        }

        emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));
        emitter.Emit(new Scalar(null, "type"));
        emitter.Emit(new Scalar(null, dt.ToString()));
        foreach (var scalar in scalars)
        {
            emitter.Emit(scalar);
        }

        emitter.Emit(new MappingEnd());
    }

    // private IEnumerable<Scalar> GetScalars(EnumDataType dt)
    // {
    //     if (string.IsNullOrEmpty(dt.EnumTypeName))
    //     {
    //         yield break;
    //     }
    //
    //     yield return new(null, "typeName");
    //     yield return new(null, dt.EnumTypeName);
    // }
    //
    // private IEnumerable<Scalar> GetScalars(ObjectDataType dt)
    // {
    //     if (string.IsNullOrEmpty(dt.DataTypeTypeName))
    //     {
    //         yield break;
    //     }
    //
    //     yield return new(null, "typeName");
    //     yield return new(null, dt.DataTypeTypeName);
    // }
    //
    // private IEnumerable<Scalar> GetScalars(StringDataType dt)
    // {
    //     if (dt.SupportUnicode)
    //     {
    //         yield return new(null, "unicode");
    //         yield return new(null, "true");
    //     }
    //
    //     if (dt.MaxLength.HasValue)
    //     {
    //         yield return new(null, "maxLength");
    //         yield return new(null, dt.MaxLength.Value.ToString());
    //     }
    //
    //     if (!dt.MinLength.HasValue)
    //     {
    //         yield break;
    //     }
    //
    //     yield return new(null, "minLength");
    //     yield return new(null, dt.MinLength.Value.ToString());
    // }
    //
    // private IEnumerable<Scalar> GetScalars(DecimalDataType dt)
    // {
    //     if (dt.Precision.HasValue)
    //     {
    //         yield return new(null, "precision");
    //         yield return new(null, dt.Precision.Value.ToString());
    //     }
    //
    //     if (!dt.Scale.HasValue)
    //     {
    //         yield break;
    //     }
    //
    //     yield return new(null, "scale");
    //     yield return new(null, dt.Scale.Value.ToString());
    // }
}