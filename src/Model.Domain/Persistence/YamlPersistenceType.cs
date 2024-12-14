using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Model.Domain;

public record YamlPersistenceType() : FilePersistenceType<Settings>("Yaml")
{
    public override string Serialize(Settings settings) =>
        new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build()
            .Serialize(settings);

    public override Settings Deserialize(string content) =>
        new DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new ImmutableListTypeConverter())
            .Build()            
            .Deserialize<Settings>(content) ?? Settings.Default;

    private class ImmutableListTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableList<>);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var itemType = type.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(itemType);
            var deserializer = new DeserializerBuilder().Build();

            // Deserialize into a mutable list first
            var list = (IList?)deserializer.Deserialize(parser, listType);

            // Convert to ImmutableList
            var toImmutableListMethod = typeof(ImmutableList)
                .GetMethods()
                .First(m => m is { Name: "ToImmutableList", IsGenericMethod: true } && m.GetParameters().Length == 1)
                .MakeGenericMethod(itemType);

            return toImmutableListMethod?.Invoke(null, new [] { list });
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var itemType = type.GetGenericArguments()[0];
            var enumerable = (IEnumerable)value;

            var serializer = new SerializerBuilder()
                .Build();

            // Serialize the list as a normal collection
            serializer.Serialize(emitter, enumerable, typeof(IEnumerable<>).MakeGenericType(itemType));
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}