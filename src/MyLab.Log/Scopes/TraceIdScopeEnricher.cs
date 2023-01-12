using System.Collections.Generic;
using System.Linq;

namespace MyLab.Log.Scopes
{
    class TraceIdScopeEnricher : ScopeEnricher
    {
        public TraceIdScopeEnricher()
        {
            InterruptAfterSuccess = true;
        }

        public override bool TryEnrich(object scope, LogEntity logEntity)
        {
            string foundTraceId = null;

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

            if (foundTraceId != null)
            {
                logEntity.Facts.Add(PredefinedFacts.TraceId, foundTraceId);
                return true;
            }

            return false;
        }
    }
}