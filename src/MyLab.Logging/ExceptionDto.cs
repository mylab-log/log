using System;
using System.Linq;
using YamlDotNet.Serialization;

namespace MyLab.Logging
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
        public string Message { get; set; }
        /// <summary>
        /// Stack trace
        /// </summary>
        [YamlMember(Order = 3)]
        public string StackTrace { get; set; }
        /// <summary>
        /// .NET type
        /// </summary>
        [YamlMember(Order = 1)]
        public string Type { get; set; }
        /// <summary>
        /// Array of aggregated exceptions when origin exception is <see cref="AggregateException"/>
        /// </summary>
        [YamlMember(Order = 6)]
        public ExceptionDto[] Aggregated { get; set; }
        /// <summary>
        /// Inner exception
        /// </summary>
        [YamlMember(Order = 5)]
        public ExceptionDto Inner { get; set; }
        /// <summary>
        /// Exception facts
        /// </summary>
        [YamlMember(Order = 4)]
        public LogFacts Facts{ get; set; }
        /// <summary>
        /// Exception labels
        /// </summary>
        [YamlMember(Order = 2)]
        public LogLabels Labels{ get; set; }

        /// <summary>
        /// Creates <see cref="ExceptionDto"/> from <see cref="Exception"/>
        /// </summary>
        public static ExceptionDto Create(Exception e)
        {
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

            dto.Facts = new LogFacts(e.GetFacts());
            dto.Labels= new LogLabels(e.GetLabels());

            return dto;
        }
    }
}