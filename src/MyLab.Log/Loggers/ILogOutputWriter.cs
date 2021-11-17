using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    interface ILogOutputWriter
    {
        void WriteLine(string text, LogLevel logLevel);
    }
}