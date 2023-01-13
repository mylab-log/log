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
    class MyLabConsoleFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable _optionsReloadToken;
        private MyLabFormatterOptions _formatterOptions;

        private readonly ScopeEnricher[] _scopeEnrichers;
        private readonly IncludeAllScopesEnricher _includeAllScopesEnricher;

        public MyLabConsoleFormatter(IOptionsMonitor<MyLabFormatterOptions> options) : base("mylab")
        {
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            _formatterOptions = options.CurrentValue;

            _includeAllScopesEnricher = new IncludeAllScopesEnricher
            {
                IncludeScopesOption = _formatterOptions.IncludeScopes
            };

            _scopeEnrichers = new ScopeEnricher[]
            {
                new TraceIdScopeEnricher(),
                new FactScopeEnricher(),
                _includeAllScopesEnricher
            };
        }

        private void ReloadLoggerOptions(MyLabFormatterOptions newOpts)
        {
            _formatterOptions = newOpts;

            _includeAllScopesEnricher.IncludeScopesOption = _formatterOptions.IncludeScopes;
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
            _formatterOptions.DebugWriter?.WriteLine(logString);
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

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
