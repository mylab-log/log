using System;
using MyLab.Logging.Serializing;

namespace MyLab.Logging
{
    /// <summary>
    /// Converts <see cref="LogEntity"/> to string
    /// </summary>
    public static class LogEntityFormatter
    {/// <summary>
        /// Formatter function
        /// </summary>
        public static readonly Func<LogEntity, Exception, string> Func = (entity, exception) => entity.ToYaml();
    }
}