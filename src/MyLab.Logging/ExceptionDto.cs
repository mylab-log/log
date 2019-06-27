using System;
using System.Linq;

namespace MyLab.Logging
{
    /// <summary>
    /// Exception log model
    /// </summary>
    public class ExceptionDto
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Stack trace
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// .NET type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Array of aggregated exceptions when origin exception is <see cref="AggregateException"/>
        /// </summary>
        public ExceptionDto[] Aggregated { get; set; }
        /// <summary>
        /// Inner exception
        /// </summary>
        public ExceptionDto Inner { get; set; }
        /// <summary>
        /// Exception conditions
        /// </summary>
        public ExceptionCondition[] Conditions { get; set; }
        /// <summary>
        /// Markers from exception
        /// </summary>
        public string[] Markers { get; set; }

        /// <summary>
        /// Creates <see cref="ExceptionDto"/> from <see cref="Exception"/>
        /// </summary>
        public static ExceptionDto Create(Exception e)
        {
            var dto = new ExceptionDto
            {
                Message = e.Message,
                StackTrace = e.StackTrace,
                Type = e.GetType().FullName
            };

            if (e is AggregateException ae)
                dto.Aggregated = ae.InnerExceptions.Select(Create).ToArray();
            
            if (e.InnerException != null)
                dto.Inner = Create(e.InnerException);

            dto.Conditions = e.GetConditions().ToArray();
            dto.Markers = e.GetMarkers().ToArray();

            return dto;
        }
    }
}