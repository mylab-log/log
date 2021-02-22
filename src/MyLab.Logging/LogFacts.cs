using System.Collections.Generic;

namespace MyLab.Logging
{
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
}