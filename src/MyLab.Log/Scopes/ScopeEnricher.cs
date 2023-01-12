using System.Collections.Generic;

namespace MyLab.Log.Scopes
{
    internal abstract class ScopeEnricher
    {
        public abstract void Enrich(IEnumerable<object> scopes, LogEntity logEntity);
    }
}
