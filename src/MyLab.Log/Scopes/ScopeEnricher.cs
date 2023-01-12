namespace MyLab.Log.Scopes
{
    internal abstract class ScopeEnricher
    {
        public bool InterruptAfterSuccess { get; protected set; } = false;
        public abstract bool TryEnrich(object scope, LogEntity logEntity);
    }
}
