using System;
using System.Collections.Generic;

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
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Log message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Facts
        /// </summary>
        public LogFacts Facts { get; }

        /// <summary>
        /// Labels
        /// </summary>
        public LogLabels Labels { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntity"/>
        /// </summary>
        public LogEntity()
        {
            Facts = new LogFacts();
            Labels = new LogLabels();
        }
    }

    /// <summary>
    /// Stores log facts
    /// </summary>
    public class LogFacts : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LogFacts"/>
        /// </summary>
        public LogFacts()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogFacts"/>
        /// </summary>
        public LogFacts(Dictionary<string, object> init)
            :base(init)
        {
            
        }
    }

    /// <summary>
    /// Stores log labels
    /// </summary>
    public class LogLabels : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LogLabels"/>
        /// </summary>
        public LogLabels()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogLabels"/>
        /// </summary>
        public LogLabels(Dictionary<string, string> init)
            : base(init)
        {

        }
    }
}