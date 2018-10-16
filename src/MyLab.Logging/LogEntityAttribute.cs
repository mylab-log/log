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
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets attribute value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="LogEntityAttribute"/>
        /// </summary>
        public LogEntityAttribute()
        {
            
        }

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