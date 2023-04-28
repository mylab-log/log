using System.Collections.Generic;
using System.Linq;

namespace MyLab.Log.Scopes
{
    class IncludeAllScopesEnricher : ScopeEnricher
    {
        public bool IncludeScopesOption { get; set; }

        public override void Enrich(IEnumerable<object> scopes, LogEntity logEntity)
        {
            if(!IncludeScopesOption) return;

            var scopesFact = scopes.OfType<IEnumerable<KeyValuePair<string, object>>>()
                .ToDictionary(sc => sc.GetType().Name, sc => sc);
            
            if (scopesFact.Count != 0 && !logEntity.Facts.ContainsKey(PredefinedFacts.Scopes))
            {
                logEntity.Facts.Add(PredefinedFacts.Scopes, scopesFact);
            }
        }
    }
}