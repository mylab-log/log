namespace MyLab.Log
{
    /// <summary>
    /// Predefined log levels
    /// </summary>
    public static class PredefinedLogLevels
    {
        /// <summary>
        /// Error
        /// </summary>
        public const string Error = "error";
        /// <summary>
        /// Debug
        /// </summary>
        public const string Debug = "debug";
        /// <summary>
        /// Warning
        /// </summary>
        public const string Warning = "warning";
    }

    /// <summary>
    /// Contains predefined log fact keys
    /// </summary>
    public static class PredefinedFacts
    {       
        /// <summary>
        /// Log conditions fact key
        /// </summary>
        public const string Conditions = "conditions";
    }

    /// <summary>
    /// Contains predefined log labels name
    /// </summary>
    public static class PredefinedLabels
    {
        /// <summary>
        /// Log level label. Predefined values keeps in  <see cref="PredefinedLogLevels"/> 
        /// </summary>
        public const string LogLevel = "log_level";
    }
}