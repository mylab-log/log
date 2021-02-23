using System;

namespace MyLab.Log.Serializing
{
    /// <summary>
    /// Contains extension serialization methods for <see cref="LogEntity"/>
    /// </summary>
    public static class LogEntitySerializationExtensions
    {
        static readonly YamlLogEntitySerializer YamlSerializer = new YamlLogEntitySerializer();
        static readonly JsonLogEntitySerializer JsonSerializer = new JsonLogEntitySerializer();

        public static string ToYaml(this LogEntity logEntity)
        {
            if (logEntity == null) throw new ArgumentNullException(nameof(logEntity));
            return YamlSerializer.Serialize(logEntity);
        }

        public static string ToJson(this LogEntity logEntity)
        {
            if (logEntity == null) throw new ArgumentNullException(nameof(logEntity));
            return JsonSerializer.Serialize(logEntity);
        }
    }
}
