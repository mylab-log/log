using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLab.Logging
{
    /// <summary>
    /// Extensions for <see cref="System.Exception" />
    /// </summary>
    public static class ExceptionExtensions
    {
        private const string ConditionsKey = "conditions";
        private const string MarkersKey = "markers";

        /// <summary>Adds condition</summary>
        public static Exception AndFactIs(this Exception exception, string key, object value)
        {
            List<ExceptionCondition> exceptionConditionList;
            if (exception.Data.Contains(ConditionsKey))
            {
                exceptionConditionList = (List<ExceptionCondition>)exception.Data[ConditionsKey];
            }
            else
            {
                exceptionConditionList = new List<ExceptionCondition>();
                exception.Data.Add((object)ConditionsKey, exceptionConditionList);
            }
            exceptionConditionList.Add(new ExceptionCondition(key, value));
            return exception;
        }

        /// <summary>Gets conditions for Exception</summary>
        public static IEnumerable<ExceptionCondition> GetConditions(
            this Exception exception)
        {
            if (exception.Data.Contains(ConditionsKey))
                return (IEnumerable<ExceptionCondition>)exception.Data[ConditionsKey];
            return Enumerable.Empty<ExceptionCondition>();
        }

        /// <summary>Adds marker for exception</summary>
        public static Exception AndMarkAs(this Exception exception, string marker)
        {
            List<string> stringList;
            if (exception.Data.Contains(MarkersKey))
            {
                stringList = (List<string>)exception.Data[MarkersKey];
            }
            else
            {
                stringList = new List<string>();
                exception.Data.Add(MarkersKey, stringList);
            }
            stringList.Add(marker);
            return exception;
        }

        /// <summary>Gets markers for Exception</summary>
        public static IEnumerable<string> GetMarkers(this Exception exception)
        {
            if (exception.Data.Contains(MarkersKey))
                return (IEnumerable<string>)exception.Data[MarkersKey];
            return Enumerable.Empty<string>();
        }
    }
}
