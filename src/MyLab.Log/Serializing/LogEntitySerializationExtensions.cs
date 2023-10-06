using System;
using MyLab.Log.Serializing.Json;
using MyLab.Log.Serializing.Yaml;

namespace MyLab.Log.Serializing
{
    /// <summary>
    /// Contains extension serialization methods for <see cref="LogEntity"/>
    /// </summary>
    public static class LogEntitySerializationExtensions
    {
        static readonly YamlLogEntitySerializer YamlSerializer = new YamlLogEntitySerializer();
        static readonly JsonLogEntitySerializer JsonSerializer = new JsonLogEntitySerializer();

        /// <summary>
        /// Serilaizes  <see cref="LogEntity"/> with yaml format
        /// </summary>
        public static string ToYaml(this LogEntity logEntity)
        {
            if (logEntity == null) throw new ArgumentNullException(nameof(logEntity));
            return YamlSerializer.Serialize(logEntity);
        }

        /// <summary>
        /// Serilaizes  <see cref="LogEntity"/> with json format
        /// </summary>
        public static string ToJson(this LogEntity logEntity)
        {
            if (logEntity == null) throw new ArgumentNullException(nameof(logEntity));
            return JsonSerializer.Serialize(logEntity);
        }
    }
}
