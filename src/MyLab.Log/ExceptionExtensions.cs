using System;

namespace MyLab.Log
{
    /// <summary>
    /// Logging extensions for <see cref="Exception"/>
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Adds log fact
        /// </summary>
        public static TException AndFactIs<TException>(this TException exception, string fact, object value) where TException : Exception
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            new ExceptionLogData(exception).AddFact(fact, value);
            return exception;
        }

        /// <summary>
        /// Adds log label
        /// </summary>
        public static TException AddLabel<TException>(this TException exception, string label) where TException : Exception
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            new ExceptionLogData(exception).AddLabel(label, "true");
            return exception;
        }

        /// <summary>
        /// Adds log label
        /// </summary>
        public static TException AddLabel<TException>(this TException exception, string label, string value) where TException : Exception
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            new ExceptionLogData(exception).AddFact(label, value);
            return exception;
        }
    }
}
