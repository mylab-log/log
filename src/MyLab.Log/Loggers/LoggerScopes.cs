using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyLab.Log.Loggers
{
    class LoggerScopes
    {
        readonly AsyncLocal<ConcurrentDictionary<string, object>> _scopes = new AsyncLocal<ConcurrentDictionary<string, object>>();
        
        public IDisposable Add(object state)
        {
            if (_scopes.Value == null)
                _scopes.Value = new ConcurrentDictionary<string, object>();

            if (state != null)
            {
                var key = Guid.NewGuid().ToString("N");
                _scopes.Value.TryAdd(key, state);

                return new ScopeRemover(key, _scopes.Value);
            }

            return null;
        }

        public IReadOnlyDictionary<string, object> GetScopes()
        {
            var dict = new Dictionary<string, object>();

            if (_scopes.Value != null)
            {
                foreach (var scope in _scopes.Value)
                {
                    if (scope.Value is IEnumerable<KeyValuePair<string, object>> list)
                    {
                        dict.Add(scope.Value.GetType().Name, list.ToDictionary(l => l.Key, l => l.Value));
                    }
                    else
                    {
                        dict.Add(scope.Value.GetType().Name, scope.Value);
                    }
                }
            }

            return dict;
        }

        class ScopeRemover : IDisposable
        {
            private readonly string _key;
            private readonly IDictionary<string, object> _scopes;

            public ScopeRemover(string key, IDictionary<string, object> scopes)
            {
                _key = key;
                _scopes = scopes;
            }
            public void Dispose()
            {
                _scopes.Remove(_key);
            }
        }
    }
}