namespace MyLab.Logging
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
        /// Log level fact key. Predefined values keeps in  <see cref="PredefinedLogLevels"/> 
        /// </summary>
        public const string LogLevel = "log-level";

        /// <summary>
        /// Log exception fact key
        /// </summary>
        public const string Exception = "exception";

        /// <summary>
        /// Log conditions fact key
        /// </summary>
        public const string Conditions = "conditions";
    }
}