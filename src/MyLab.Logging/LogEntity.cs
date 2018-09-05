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
        public Guid Id { get; set; }
        
        /// <summary>
        /// Occurence time
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Log message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Markers
        /// </summary>
        public List<string> Markers { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public List<LogEntityAttribute> Attributes { get; set; }
    }
}