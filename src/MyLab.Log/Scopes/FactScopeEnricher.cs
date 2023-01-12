using System.Collections.Generic;

namespace MyLab.Log.Scopes
{
    class FactScopeEnricher : ScopeEnricher
    {
        public override void Enrich(IEnumerable<object> scopes, LogEntity logEntity)
        {
            foreach (var scope in scopes)
            {
                if (scope is FactLogScope factProvider)
                {
                    foreach (var factPair in factProvider)
                    {
                        logEntity.Facts.Add(factPair.Key, factPair.Value);
                    }
                }
            }
        }
    }
}