using System;
using System.Linq;
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
        
        void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            throw new NotImplementedException();
        }
        
        void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            var dto = (ExceptionDto)this;
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            if (dto.Message != null)
            {
                emitter.Emit(new Scalar(null, nameof(Message)));
                emitter.Emit(new Scalar(null, dto.Message));
            }

            if (dto.Type != null)
            {
                emitter.Emit(new Scalar(null, nameof(Type)));
                emitter.Emit(new Scalar(null, dto.Type));
            }

            if (dto.StackTrace != null)
            {
                emitter.Emit(new Scalar(null, nameof(StackTrace)));
                emitter.Emit(new Scalar(null, dto.StackTrace.Replace("\r\n", "\n")));
            }

            if (dto.Labels != null && dto.Labels.Count > 0)
            {
                emitter.Emit(new Scalar(null, nameof(Labels)));
                nestedObjectSerializer(dto.Labels);
            }

            if (dto.Facts != null && dto.Facts.Count > 0)
            {
                emitter.Emit(new Scalar(null, nameof(Facts)));
                nestedObjectSerializer(dto.Facts);
            }

            if (dto.Inner != null)
            {
                emitter.Emit(new Scalar(null, nameof(Inner)));
                nestedObjectSerializer(dto.Inner);
            }

            if (dto.Aggregated != null && dto.Aggregated.Length > 0)
            {
                emitter.Emit(new Scalar(null, nameof(ExceptionDto.Aggregated)));

                emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Any));

                foreach (var exceptionDto in dto.Aggregated)
                {
                    nestedObjectSerializer(exceptionDto);
                }

                emitter.Emit(new SequenceEnd());
            }

            emitter.Emit(new MappingEnd());
        }
    }
}