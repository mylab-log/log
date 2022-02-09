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

            ExtractReqIdAndTraceId(scopeProvider, out string reqId, out string traceId);

            if(reqId != null)
                logEntity.Facts.Add(PredefinedFacts.RequestId, reqId);
            if (traceId != null)
                logEntity.Facts.Add(PredefinedFacts.TraceId, traceId);

            var logString = resultFormatter.DynamicInvoke(logEntity, exception).ToString();

            textWriter.WriteLine(logString);
            _formatterOptions.DebugWriter?.WriteLine(logString);
        }

        void ExtractReqIdAndTraceId(IExternalScopeProvider esProvider, out string reqId, out string traceId)
        {
            var list = new List<KeyValuePair<string, object>>();

            string localReqId = null;
            string localTraceId = null;

            esProvider.ForEachScope((scope, state) =>
            {
                var scopeTypeName = scope?.GetType().Name;

                if ((scopeTypeName == "HostingLogScope" || scopeTypeName == "ActionLogScope") && scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    var items = scopeItems.ToArray();

                    var scopeTmpReqId = items
                        .FirstOrDefault(itm => itm.Key == "RequestId")
                        .Value?
                        .ToString();
                    if (!string.IsNullOrEmpty(scopeTmpReqId))
                        localReqId = scopeTmpReqId;

                    var scopeTmpTraceId = items
                        .FirstOrDefault(itm => itm.Key == "TraceId")
                        .Value?
                        .ToString();
                    if (!string.IsNullOrEmpty(scopeTmpTraceId))
                        localTraceId = scopeTmpTraceId;
                }
            }, list);

            reqId = localReqId;
            traceId = localTraceId;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
