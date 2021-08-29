using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    /// <summary>
    /// Serializes <see cref="LogEntity"/> into YAML format
    /// </summary>
    public class YamlLogEntitySerializer : ILogEntitySerializer
    {
        private readonly ISerializer _serializer;

        public YamlLogEntitySerializer()
        {
            _serializer = new SerializerBuilder()
                .WithTypeInspector(inspector => new PropertyExceptionWrapper(inspector))
                .WithTypeConverter(new LogStringValueConverter())
                .WithTypeConverter(new DateTimeValueConverter())
                .WithEventEmitter(nextEmitter => new NullStringsEventEmitter(nextEmitter))
                .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();
        }

        public string Serialize(LogEntity logEntity)
        {
            return _serializer.Serialize(logEntity);
        }
    }
}
