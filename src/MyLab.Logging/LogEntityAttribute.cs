namespace MyLab.Logging
{
    /// <summary>
    /// Contains named data
    /// </summary>
    public class LogEntityAttribute
    {
        /// <summary>
        /// Gets or sets attribute name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets or sets attribute value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntityAttribute"/>
        /// </summary>
        public LogEntityAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}