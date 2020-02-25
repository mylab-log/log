namespace MyLab.Logging
{
    /// <summary>Contains condition for exception</summary>
    public class ExceptionCondition
    {
        /// <summary>Key</summary>
        public string Key { get; }

        /// <summary>Value</summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionCondition" />
        /// </summary>
        public ExceptionCondition(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
