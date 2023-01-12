﻿using System.Collections.Generic;
using System.Linq;

namespace MyLab.Log.Scopes
{
    class TraceIdScopeEnricher : ScopeEnricher
    {
        public override void Enrich(IEnumerable<object> scopes, LogEntity logEntity)
        {
            foreach (var scope in scopes)
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    var items = scopeItems.ToArray();

                    var foundTraceId = items
                        .FirstOrDefault(itm => itm.Key == "TraceId")
                        .Value?
                        .ToString();

                    if (foundTraceId != null)
                    {
                        logEntity.Facts.Add(PredefinedFacts.TraceId, foundTraceId);
                        break;
                    }
                }
            }
        }
    }
}