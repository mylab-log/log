using System;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log
{
    /// <summary>
    /// Contains log item data
    /// </summary>
    public class LogEntity : IYamlConvertible
    {
        private ExceptionDto _exception;

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
        public ExceptionDto Exception
        {
            get => _exception;
            set
            {
                _exception = value;

                if (Labels.ContainsKey(PredefinedLabels.ExceptionTrace))
                {
                    if (value != null)
                        Labels[PredefinedLabels.ExceptionTrace] = value.ExceptionTrace;
                    else
                    {
                        Labels.Remove(PredefinedLabels.ExceptionTrace);
                    }
                }
                else if (value != null)
                {
                    Labels[PredefinedLabels.ExceptionTrace] = value.ExceptionTrace;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntity"/>
        /// </summary>
        public LogEntity()
        {
            Facts = new LogFacts();
            Labels = new LogLabels();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntity"/>
        /// </summary>
        public LogEntity(LogEntity origin)
        {
            Facts = new LogFacts(origin.Facts);
            Labels = new LogLabels(origin.Labels);
            Time = origin.Time;
            Message = origin.Message;
            Exception = origin.Exception;
        }

        /// <inheritdoc />
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            if (Message != null)
            {
                emitter.Emit(new Scalar(null, nameof(Message)));
                emitter.Emit(new Scalar(null, Message));
            }

            if (Time != default)
            {
                emitter.Emit(new Scalar(null, nameof(Time)));
                nestedObjectSerializer(Time);
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

            if (Exception != null)
            {
                emitter.Emit(new Scalar(null, nameof(Exception)));
                nestedObjectSerializer(Exception);
            }

            emitter.Emit(new MappingEnd());
        }
    }
}