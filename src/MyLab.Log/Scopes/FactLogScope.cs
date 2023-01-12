using System.Collections;
using System.Collections.Generic;

namespace MyLab.Log.Scopes
{
    /// <summary>
    /// Contains log facts from log scope
    /// </summary>
    public class FactLogScope : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _facts;

        /// <summary>
        /// Initializes a new instance of <see cref="FactLogScope"/>
        /// </summary>
        public FactLogScope(IDictionary<string,object> facts)
        {
            _facts = new Dictionary<string,object> (facts);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _facts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
