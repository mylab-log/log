using System;
using Microsoft.Extensions.Logging;

namespace MyLab.Log.Loggers
{
    class MyLabLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ILogOutputWriter _logOutputWriter;
        private readonly LoggerScopes _loggerScopes;

        public MyLabLogger(string categoryName, ILogOutputWriter logOutputWriter, LoggerScopes loggerScopes)
        {
            _categoryName = categoryName;
            _logOutputWriter = logOutputWriter ?? throw new ArgumentNullException(nameof(logOutputWriter));
            _loggerScopes = loggerScopes;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogEntity logEntity;
            Delegate resultFormatter;

            if (state is LogEntity le)
            {
                logEntity = new LogEntity(le);
                resultFormatter = formatter;
            }
            else
            {
                logEntity = new LogEntity
                {
                    Message = formatter(state, exception),
                    Time = DateTime.Now
                };

                if (exception != null)
                    logEntity.Exception = ExceptionDto.Create(exception);

                resultFormatter = LogEntityFormatter.Yaml;
            }

            if (_categoryName != null)
            {
                logEntity.Facts.Add(PredefinedFacts.Category, _categoryName);
            }

            var scopes = _loggerScopes.GetScopes();

            if (scopes.Count != 0)
            {
                logEntity.Facts.Add(PredefinedFacts.Scopes, scopes);
            }

            var logString = resultFormatter.DynamicInvoke(logEntity, exception);
            _logOutputWriter.WriteLine(logString.ToString(), logLevel);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _loggerScopes.Add(state);
        }
    }
}