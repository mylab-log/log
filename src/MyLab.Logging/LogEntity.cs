using System;
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
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Log message
        /// </summary>
        [YamlMember(Order = 0)]
        public string Content { get; set; }

        /// <summary>
        /// Facts
        /// </summary>
        [YamlMember(Order = 3)]
        public LogFacts Facts { get; }

        /// <summary>
        /// Labels
        /// </summary>
        [YamlMember(Order = 2)]
        public LogLabels Labels { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntity"/>
        /// </summary>
        public LogEntity()
        {
            Facts = new LogFacts();
            Labels = new LogLabels();
        }

        /// <summary>
        /// Sets information about Exception
        /// </summary>
        public void SetException(Exception e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            if (Facts.ContainsKey(PredefinedFacts.Exception))
            {
                Facts[PredefinedFacts.Exception] = ExceptionDto.Create(e);
            }
            else
            {
                Facts.Add(PredefinedFacts.Exception, ExceptionDto.Create(e));
            }
        }
    }
}