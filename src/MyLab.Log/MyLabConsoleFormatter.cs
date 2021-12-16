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

            string tmpReqId = null;
            string tmpTraceId = null;

            esProvider.ForEachScope((scope, state) =>
            {
                if (scope?.GetType().Name == "HostingLogScope" && scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    var items = scopeItems.ToArray();
                    tmpReqId = items.FirstOrDefault(itm => itm.Key == "RequestId").Value?.ToString();
                    tmpTraceId = items.FirstOrDefault(itm => itm.Key == "TraceId").Value?.ToString();
                }
            }, list);

            reqId = tmpReqId;
            traceId = tmpTraceId;
        }

        IReadOnlyDictionary<string, object> GetScopesObject(IExternalScopeProvider esProvider)
        {
            var list = new List<KeyValuePair<string, object>>();
            esProvider.ForEachScope((scope, state) =>
            {
                if (scope != null)
                {
                    state.Add(new KeyValuePair<string, object>(scope.GetType().Name, scope));
                }
            }, list);

            return list.GroupBy(itm => itm.Key, pair => pair.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count() == 1 ? g.First() : g.ToArray()
                    );
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
