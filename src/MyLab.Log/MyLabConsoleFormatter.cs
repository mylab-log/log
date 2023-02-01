using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using MyLab.Log.Scopes;

namespace MyLab.Log
{
    class MyLabConsoleFormatter : ConsoleFormatter
    {
        private readonly MyLabFormatterOptions _options;
        private readonly ScopeEnricher[] _scopeEnrichers;

        public MyLabConsoleFormatter(
            IOptions<ConsoleFormatterOptions> consoleOptions,
            IOptions<MyLabFormatterOptions> mylabOptions
        ) : base("mylab")
        {
            _options = mylabOptions.Value.JoinConsoleFormatterOptions(consoleOptions.Value);
            _scopeEnrichers = new ScopeEnricher[]
            {
                new TraceIdScopeEnricher(),
                new FactScopeEnricher(),
                new LabelScopeEnricher(),
                new IncludeAllScopesEnricher
                {
                    IncludeScopesOption = _options.IncludeScopes
                }
            };
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

            if (scopeProvider != null)
            {
                EnrichLogEntityFromScope(scopeProvider, logEntity);
            }

            var logString = resultFormatter.DynamicInvoke(logEntity, exception).ToString();

            textWriter.WriteLine(logString);
            _options.DebugWriter?.WriteLine(logString);
        }

        private void EnrichLogEntityFromScope(IExternalScopeProvider scopeProvider, LogEntity logEntity)
        {
            var scopes = new List<object>();

            scopeProvider.ForEachScope((scope, state) =>
            {
                state.Add(scope);
            }, scopes);

            foreach (var scopeEnricher in _scopeEnrichers)
            {
                scopeEnricher.Enrich(scopes, logEntity);
            }
        }

        private void EnrichFromHttpScope(IExternalScopeProvider scopeProvider, LogEntity logEntity)
        {
            var traceId = ExtractTraceId(scopeProvider);

            if (traceId != null)
                logEntity.Facts.Add(PredefinedFacts.TraceId, traceId);
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
    }
}
