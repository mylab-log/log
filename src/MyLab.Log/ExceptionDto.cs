using System;
using System.Linq;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace MyLab.Log
{
    /// <summary>
    /// Exception log model
    /// </summary>
    public class ExceptionDto
    {
        /// <summary>
        /// Message
        /// </summary>
        [YamlMember(Order = 0)]
        [JsonProperty(Order = 0)]
        public string Message { get; set; }
        /// <summary>
        /// Stack trace
        /// </summary>
        [YamlMember(Order = 2)]
        [JsonProperty(Order = 2)]
        public string StackTrace { get; set; }
        /// <summary>
        /// .NET type
        /// </summary>
        [YamlMember(Order = 1)]
        [JsonProperty(Order = 1)]
        public string Type { get; set; }
        /// <summary>
        /// Array of aggregated exceptions when origin exception is <see cref="AggregateException"/>
        /// </summary>
        [YamlMember(Order = 6)]
        [JsonProperty(Order = 6)]
        public ExceptionDto[] Aggregated { get; set; }
        /// <summary>
        /// Inner exception
        /// </summary>
        [YamlMember(Order = 5)]
        [JsonProperty(Order = 5)]
        public ExceptionDto Inner { get; set; }
        /// <summary>
        /// Exception facts
        /// </summary>
        [YamlMember(Order = 4)]
        [JsonProperty(Order = 4)]
        public LogFacts Facts{ get; set; }
        /// <summary>
        /// Exception labels
        /// </summary>
        [YamlMember(Order = 3)]
        [JsonProperty(Order = 3)]
        public LogLabels Labels{ get; set; }

        /// <summary>
        /// Creates <see cref="ExceptionDto"/> from <see cref="Exception"/>
        /// </summary>
        public static ExceptionDto Create(Exception e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var dto = new ExceptionDto
            {
                Message = e.Message,
                StackTrace = e.StackTrace,
                Type = e.GetType().FullName
            };

            if (e is AggregateException ae)
                dto.Aggregated = ae.InnerExceptions.Select(Create).ToArray();
            
            if (e.InnerException != null)
                dto.Inner = Create(e.InnerException);

            var eLogData = new ExceptionLogData(e);
            dto.Facts = new LogFacts(eLogData.GetFacts());
            dto.Labels= new LogLabels(eLogData.GetLabels());

            return dto;
        }

        public static implicit operator ExceptionDto(Exception e)
        {
            if (e == null) return null;
            return Create(e);
        }
    }
}