using System;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    /// <summary>
    /// Serializes <see cref="LogEntity"/> into YAML format
    /// </summary>
    public class YamlLogEntitySerializer : ILogEntitySerializer
    {
        private readonly ISerializer _serializer;

        /// <summary>
        /// Initialize a new instance of <see cref="YamlLogEntitySerializer"/>
        /// </summary>
        public YamlLogEntitySerializer()
        {
            _serializer = new SerializerBuilder()
                .WithTypeInspector(inspector => new PropertyExceptionWrapper(inspector))
                .WithTypeConverter(new LogStringValueConverter())
                .WithTypeConverter(new DateTimeValueConverter())
                .WithTypeConverter(new ReflectionConverter())
                .WithTypeConverter(new JTokenConverter())
                .WithTypeConverter(new ByteReadonlyMemoryConverter())
                .WithEventEmitter(nextEmitter => new NullStringsEventEmitter(nextEmitter))
                .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();
        }

        /// <inheritdoc />
        public string Serialize(LogEntity logEntity)
        {
            try
            {
                return _serializer.Serialize(logEntity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
