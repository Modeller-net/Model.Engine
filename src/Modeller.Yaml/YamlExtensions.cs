using YamlDotNet.Serialization.NamingConventions;

namespace Modeller.Yaml;

public static class YamlExtensions
{
    public static string ToYaml<T>(this T obj, bool includeNull = true)
    {
        var serialiser = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlNameConverter())
            .WithTypeConverter(new YamlDataTypeConverter())
            .Build();
        return serialiser.Serialize(obj);
    }

    public static T FromYaml<T>(this string yaml)
    {
        var deserialiser = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlNameConverter())
            .WithTypeConverter(new YamlDataTypeConverter())
            .Build();
        return deserialiser.Deserialize<T>(yaml);
    }
}