using System.Collections.Generic;

namespace MyLab.Log.Scopes
{
    class LabelScopeEnricher : ScopeEnricher
    {
        public override void Enrich(IEnumerable<object> scopes, LogEntity logEntity)
        {
            foreach (var scope in scopes)
            {
                if (scope is LabelLogScope labelProvider)
                {
                    foreach (var labelPair in labelProvider)
                    {
                        if (!logEntity.Labels.ContainsKey(labelPair.Key))
                        {
                            logEntity.Labels.Add(labelPair.Key, labelPair.Value?.ToString());
                        }
                    }
                }
            }
        }
    }
}