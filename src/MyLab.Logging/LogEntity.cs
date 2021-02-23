using System;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace MyLab.Logging
{
    /// <summary>
    /// Contains log item data
    /// </summary>
    public class LogEntity
    {
        /// <summary>
        /// Occurrence time
        /// </summary>
        [YamlMember(Order = 1)]
        [JsonProperty(Order = 1)]
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Log message
        /// </summary>
        [YamlMember(Order = 0)]
        [JsonProperty(Order = 0)]
        public string Message { get; set; }

        /// <summary>
        /// Facts
        /// </summary>
        [YamlMember(Order = 3)]
        [JsonProperty(Order = 3)]
        public LogFacts Facts { get; }

        /// <summary>
        /// Labels
        /// </summary>
        [YamlMember(Order = 2)]
        [JsonProperty(Order = 2)]
        public LogLabels Labels { get; }

        /// <summary>
        /// Contains exception info
        /// </summary>
        [YamlMember(Order = 4)]
        [JsonProperty(Order = 4)]
        public ExceptionDto Exception { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntity"/>
        /// </summary>
        public LogEntity()
        {
            Facts = new LogFacts();
            Labels = new LogLabels();
        }
    }
}