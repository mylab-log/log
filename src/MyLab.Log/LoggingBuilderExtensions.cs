using Microsoft.Extensions.Logging;
using MyLab.Log.Loggers;

namespace MyLab.Log
{
    /// <summary>
    /// Contains extensions for <see cref="ILoggingBuilder"/>
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Adds MyLab console logger
        /// </summary>
        public static ILoggingBuilder AddMyLabConsole(this ILoggingBuilder loggingBuilder)
        {
            return loggingBuilder.AddProvider(new MyLabConsoleLoggerProvider());
        }

        /// <summary>
        /// Adds MyLab debug logger
        /// </summary>
        public static ILoggingBuilder AddMyLabDebug(this ILoggingBuilder loggingBuilder)
        {
            return loggingBuilder.AddProvider(new MyLabDebugLoggerProvider());
        }
    }
}
