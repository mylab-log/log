using System.IO;
using Microsoft.Extensions.Logging;

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
            return loggingBuilder.AddConsole(o => o.FormatterName = "mylab")
                .AddConsoleFormatter<MyLabConsoleFormatter, MyLabFormatterOptions>();
        }
        
        internal static ILoggingBuilder AddMyLabFormatter(this ILoggingBuilder loggingBuilder, TextWriter debugWriter)
        {
            return loggingBuilder.AddConsoleFormatter<MyLabConsoleFormatter, MyLabFormatterOptions>(o => o.DebugWriter = debugWriter);
        }
    }
}
