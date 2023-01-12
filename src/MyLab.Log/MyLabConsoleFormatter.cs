using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace MyLab.Log
{
    class MyLabConsoleFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable _optionsReloadToken;
        private MyLabFormatterOptions _formatterOptions;

        public MyLabConsoleFormatter(IOptionsMonitor<MyLabFormatterOptions> options) : base("mylab")
        {
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            _formatterOptions = options.CurrentValue;
        }

        private void ReloadLoggerOptions(MyLabFormatterOptions newOpts)
        {
            _formatterOptions = newOpts;
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            TState state = logEntry.State;
            var formatter = logEntry.Formatter;
            var exception = logEntry.Exception;
            var categoryName = logEntry.Category;

            LogEntity logEntity;
            Delegate resultFormatter = (Delegate)formatter ?? LogEntityFormatter.Yaml;
            
            if (state is LogEntity le)
            {
                logEntity = new LogEntity(le);
            }
            else
            {
                logEntity = new LogEntity
                {
                    Message = resultFormatter.DynamicInvoke(state, exception).ToString(),
                    Time = DateTime.Now
                };

                if (exception != null)
                    logEntity.Exception = ExceptionDto.Create(exception);

                resultFormatter = LogEntityFormatter.Yaml;
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                logEntity.Facts.Add(PredefinedFacts.Category, categoryName);
            }

            var traceId = ExtractTraceId(scopeProvider);
            
            if (traceId != null)
                logEntity.Facts.Add(PredefinedFacts.TraceId, traceId);

            var logString = resultFormatter.DynamicInvoke(logEntity, exception).ToString();

            textWriter.WriteLine(logString);
            _formatterOptions.DebugWriter?.WriteLine(logString);
        }

        string ExtractTraceId(IExternalScopeProvider esProvider)
        {
            var list = new List<KeyValuePair<string, object>>();

            string foundTraceId = null;

            esProvider.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    var items = scopeItems.ToArray();
                    
                    var scopeTmpTraceId = items
                        .FirstOrDefault(itm => itm.Key == "TraceId")
                        .Value?
                        .ToString();
                    if (!string.IsNullOrEmpty(scopeTmpTraceId))
                        foundTraceId = scopeTmpTraceId;
                }
            }, list);
            
            return foundTraceId;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
