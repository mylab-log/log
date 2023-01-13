using System.IO;
using Microsoft.Extensions.Logging.Console;

namespace MyLab.Log
{
    class MyLabFormatterOptions : ConsoleFormatterOptions
    {
        public TextWriter DebugWriter { get; set; }

        public MyLabFormatterOptions JoinConsoleFormatterOptions(ConsoleFormatterOptions consoleFormatterOptions)
        {
            return new MyLabFormatterOptions
            {
                DebugWriter = DebugWriter,
                IncludeScopes = IncludeScopes || consoleFormatterOptions.IncludeScopes,
                TimestampFormat = TimestampFormat ?? consoleFormatterOptions.TimestampFormat,
                UseUtcTimestamp = UseUtcTimestamp || consoleFormatterOptions.UseUtcTimestamp,
            };
        }
    }
}
