using System;

namespace MyLab.Logging
{
    /// <summary>
    /// Extensions for <see cref="System.Exception" />
    /// </summary>
    public static class ExceptionExtensions
    {

        private const string FactsKey = "facts";
        private const string LabelsKey = "labels";

        /// <summary>
        /// Adds fact
        /// </summary>
        public static Exception AndFactIs(this Exception exception, string key, object value)
        { 
            LogFacts exceptionFacts;
            if (exception.Data.Contains(FactsKey))
            {
                exceptionFacts = (LogFacts)exception.Data[FactsKey];
            }
            else
            {
                exceptionFacts = new LogFacts();
                exception.Data.Add(FactsKey, exceptionFacts);
            }
            exceptionFacts.Add(key, value);
            return exception;
        }

        /// <summary>Gets conditions for Exception</summary>
        public static LogFacts GetFacts(this Exception exception)
        {
            if (exception.Data.Contains(FactsKey))
                return (LogFacts)exception.Data[FactsKey];
            return new LogFacts();
        }

        /// <summary>Adds marker for exception</summary>
        public static Exception AndMark(this Exception exception, string labelKey)
        {
            return AndMark(exception, labelKey, "true");
        }

        /// <summary>Adds marker for exception</summary>
        public static Exception AndMark(this Exception exception, string labelKey, string labelValue)
        {
            LogLabels stringList;
            if (exception.Data.Contains(LabelsKey))
            {
                stringList = (LogLabels)exception.Data[LabelsKey];
            }
            else
            {
                stringList = new LogLabels();
                exception.Data.Add(LabelsKey, stringList);
            }
            stringList.Add(labelKey, labelValue);
            return exception;
        }

        /// <summary>Gets markers for Exception</summary>
        public static LogLabels GetLabels(this Exception exception)
        {
            if (exception.Data.Contains(LabelsKey))
                return (LogLabels)exception.Data[LabelsKey];
            return new LogLabels();
        }
    }
}
