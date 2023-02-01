using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.Log.Scopes
{
    /// <summary>
    /// Contains log labels from log scope
    /// </summary>
    public class LabelLogScope : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _facts;

        /// <summary>
        /// Initializes a new instance of <see cref="LabelLogScope"/>
        /// </summary>
        public LabelLogScope(IDictionary<string, string> facts)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));

            _facts = facts.ToDictionary(f => f.Key, f => (object)f.Value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LabelLogScope"/>
        /// </summary>
        public LabelLogScope(string name, string value)
            : this(new Dictionary<string, string> { { name, value } })
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
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