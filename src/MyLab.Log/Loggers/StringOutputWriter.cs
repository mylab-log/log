using System.Text;
using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    class StringOutputWriter : ILogOutputWriter
    {
        private readonly StringBuilder _stringBuilder;

        public StringOutputWriter(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public void WriteLine(string text, LogLevel logLevel)
        {
            _stringBuilder.AppendLine(text);
        }
    }
}