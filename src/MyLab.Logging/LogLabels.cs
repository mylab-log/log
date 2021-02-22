using System.Collections.Generic;

namespace MyLab.Logging
{
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