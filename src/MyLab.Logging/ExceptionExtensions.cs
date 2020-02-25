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
            if (exception.Data.Contains("conditions"))
            {
                exceptionConditionList = (List<ExceptionCondition>)exception.Data["conditions"];
            }
            else
            {
                exceptionConditionList = new List<ExceptionCondition>();
                exception.Data.Add((object)"conditions", exceptionConditionList);
            }
            exceptionConditionList.Add(new ExceptionCondition(key, value));
            return exception;
        }

        /// <summary>Gets conditions for Exception</summary>
        public static IEnumerable<ExceptionCondition> GetConditions(
            this Exception exception)
        {
            if (exception.Data.Contains("conditions"))
                return (IEnumerable<ExceptionCondition>)exception.Data["conditions"];
            return Enumerable.Empty<ExceptionCondition>();
        }

        /// <summary>Adds marker for exception</summary>
        public static Exception AndMarkAs(this Exception exception, string marker)
        {
            List<string> stringList;
            if (exception.Data.Contains("markers"))
            {
                stringList = (List<string>)exception.Data["markers"];
            }
            else
            {
                stringList = new List<string>();
                exception.Data.Add("markers", stringList);
            }
            stringList.Add(marker);
            return exception;
        }

        /// <summary>Gets markers for Exception</summary>
        public static IEnumerable<string> GetMarkers(this Exception exception)
        {
            if (exception.Data.Contains("markers"))
                return (IEnumerable<string>)exception.Data["markers"];
            return Enumerable.Empty<string>();
        }
    }
}
