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
        /// Event identifier
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Event instance identifier
        /// </summary>
        public Guid InstanceId { get; set; }
        
        /// <summary>
        /// Occurence time
        /// </summary>
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Log message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Markers
        /// </summary>
        public List<string> Markers { get; } = new List<string>();

        /// <summary>
        /// Conditions with names
        /// </summary>
        public List<LogEntityCustomCondition> CustomConditions { get; } = new List<LogEntityCustomCondition>();

        /// <summary>
        /// Conditions
        /// </summary>
        public List<string> Conditions { get; } = new List<string>();
        
        /// <summary>
        /// Clones object
        /// </summary>
        public LogEntity Clone(Guid? newInstanceId = null)
        {
            var e = new LogEntity
            {
                EventId = EventId,
                InstanceId = newInstanceId ?? InstanceId,
                DateTime = DateTime,
                Message = Message
            };

            e.Markers.AddRange(Markers);
            e.Conditions.AddRange(Conditions);
            e.CustomConditions.AddRange(CustomConditions);

            return e;
        }
    }
}