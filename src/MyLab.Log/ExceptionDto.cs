using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log
{
    /// <summary>
    /// Exception log model
    /// </summary>
    public class ExceptionDto : IYamlConvertible
    {
        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty(Order = 0)]
        public string Message { get; set; }
        /// <summary>
        /// Contains exception hash 
        /// </summary>
        /// <remarks>
        /// If dto created based on exception object then this property value calculated automatically based on message, type and stacktrace (without line numbers) this exception with exception traces of aggregated and inner exceptions
        /// </remarks>
        [JsonProperty(Order = 1)]
        public string ExceptionTrace { get; set; }
        /// <summary>
        /// Exception labels
        /// </summary>
        [JsonProperty(Order = 2)]
        public LogLabels Labels { get; set; }
        /// <summary>
        /// Exception facts
        /// </summary>
        [JsonProperty(Order = 3)]
        public LogFacts Facts { get; set; }
        /// <summary>
        /// .NET type
        /// </summary>
        [JsonProperty(Order = 4)]
        public string Type { get; set; }
        /// <summary>
        /// Stack trace
        /// </summary>
        [JsonProperty(Order = 5)]
        public string StackTrace { get; set; }
        /// <summary>
        /// Array of aggregated exceptions when origin exception is <see cref="AggregateException"/>
        /// </summary>
        [JsonProperty(Order = 6)]
        public ExceptionDto[] Aggregated { get; set; }
        /// <summary>
        /// Inner exception
        /// </summary>
        [JsonProperty(Order = 7)]
        public ExceptionDto Inner { get; set; }

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

            dto.ExceptionTrace = CalcTrace(dto);

            return dto;
        }

        public static implicit operator ExceptionDto(Exception e)
        {
            if (e == null) return null;
            return Create(e);
        }

        private static string CalcTrace(ExceptionDto dto)
        {
            var traceDataBuilder = new StringBuilder();

            if (dto.Message != null) traceDataBuilder.AppendLine(dto.Message);
            if (dto.Type != null) traceDataBuilder.AppendLine(dto.Type);
            if (dto.StackTrace != null)
            {
                var normStackTrace = string.Join("", dto.StackTrace.Split('\n').Select(s =>
                {
                    var tmp = s.Trim();
                    int lineMarkerPos = tmp.LastIndexOf(":line", StringComparison.InvariantCulture);

                    return lineMarkerPos >= 0
                        ? tmp.Remove(lineMarkerPos)
                        : tmp;
                }));

                traceDataBuilder.AppendLine(normStackTrace);
            }

            if (dto.Aggregated != null)
            {
                string aggregatedTraces = string.Join(" ", dto.Aggregated.Select(e => e.ExceptionTrace).Where(t => t != null));
                traceDataBuilder.AppendLine(aggregatedTraces);
            }

            if (dto.Inner != null) traceDataBuilder.AppendLine(dto.Inner.ExceptionTrace);

            var bin = Encoding.UTF8.GetBytes(traceDataBuilder.ToString());
            var md5 = MD5.Create();

            var binHash = md5.ComputeHash(bin);

            return BitConverter.ToString(binHash).Replace("-", "").ToLower();
        }

        void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            throw new NotImplementedException();
        }
        
        void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            if (Message != null)
            {
                emitter.Emit(new Scalar(null, nameof(Message)));
                emitter.Emit(new Scalar(null, Message));
            }

            if (ExceptionTrace != null)
            {
                emitter.Emit(new Scalar(null, nameof(ExceptionTrace)));
                emitter.Emit(new Scalar(null, ExceptionTrace));
            }

            if (Type != null)
            {
                emitter.Emit(new Scalar(null, nameof(Type)));
                emitter.Emit(new Scalar(null, Type));
            }

            if (StackTrace != null)
            {
                emitter.Emit(new Scalar(null, nameof(StackTrace)));
                emitter.Emit(new Scalar(null, StackTrace.Replace("\r\n", "\n")));
            }

            if (Labels != null && Labels.Count > 0)
            {
                emitter.Emit(new Scalar(null, nameof(Labels)));
                nestedObjectSerializer(Labels);
            }

            if (Facts != null && Facts.Count > 0)
            {
                emitter.Emit(new Scalar(null, nameof(Facts)));
                nestedObjectSerializer(Facts);
            }

            if (Inner != null)
            {
                emitter.Emit(new Scalar(null, nameof(Inner)));
                nestedObjectSerializer(Inner);
            }

            if (Aggregated != null && Aggregated.Length > 0)
            {
                emitter.Emit(new Scalar(null, nameof(Aggregated)));

                emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Any));

                foreach (var exceptionDto in Aggregated)
                {
                    nestedObjectSerializer(exceptionDto);
                }

                emitter.Emit(new SequenceEnd());
            }

            emitter.Emit(new MappingEnd());
        }
    }
}