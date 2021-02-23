using System;
using MyLab.Log.Serializing;

namespace MyLab.Log
{
    /// <summary>
    /// Converts <see cref="LogEntity"/> to string
    /// </summary>
    public static class LogEntityFormatter
    {
        /// <summary>
        /// Yaml formatter function
        /// </summary>
        public static readonly Func<LogEntity, Exception, string> Yaml = (entity, exception) => entity.ToYaml();

        /// <summary>
        /// Json formatter function
        /// </summary>
        public static readonly Func<LogEntity, Exception, string> Json = (entity, exception) => entity.ToJson();
    }
}