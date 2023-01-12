namespace MyLab.Log.Scopes
{
    class FactScopeEnricher : ScopeEnricher
    {
        public override bool TryEnrich(object scope, LogEntity logEntity)
        {
            bool success = false;

            if (scope is FactLogScope factProvider)
            {
                foreach (var factPair in factProvider)
                {
                    logEntity.Facts.Add(factPair.Key, factPair.Value);

                    success = true;
                }
            }

            return success;
        }
    }
}