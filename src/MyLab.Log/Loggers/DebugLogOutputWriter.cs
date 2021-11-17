using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    class DebugLogOutputWriter : ILogOutputWriter
    {
        public void WriteLine(string text, LogLevel logLevel)
        {
            Debug.WriteLine(text);
        }
    }
}