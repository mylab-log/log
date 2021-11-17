using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    class MyLabLoggerProvider : ILoggerProvider
    {
        private readonly ILogOutputWriter _logOutputWriter;
        readonly LoggerScopes _loggerScopes = new LoggerScopes();

        public MyLabLoggerProvider(ILogOutputWriter logOutputWriter)
        {
            _logOutputWriter = logOutputWriter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLabLogger(categoryName, _logOutputWriter, _loggerScopes);
        }

        public void Dispose()
        {

        }
    }
}