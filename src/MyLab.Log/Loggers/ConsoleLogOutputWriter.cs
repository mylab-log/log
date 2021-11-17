using System;
using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    class ConsoleLogOutputWriter : ILogOutputWriter
    {
        public void WriteLine(string text, LogLevel logLevel)
        {
            var textWriter = logLevel == LogLevel.Critical || logLevel == LogLevel.Error ? Console.Error : Console.Out;

            textWriter.WriteLine(text);
        }
    }
}