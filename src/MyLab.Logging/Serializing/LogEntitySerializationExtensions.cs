using System;

namespace MyLab.Logging.Serializing
{
    /// <summary>
    /// Contains extension serialization methods for <see cref="LogEntity"/>
    /// </summary>
    public static class LogEntitySerializationExtensions
    {
        static readonly YamlLogEntitySerializer YamlSerializer = new YamlLogEntitySerializer();

        public static string ToYaml(this LogEntity logEntity)
        {
            if (logEntity == null) throw new ArgumentNullException(nameof(logEntity));
            return YamlSerializer.Serialize(logEntity);
        }
    }
}
