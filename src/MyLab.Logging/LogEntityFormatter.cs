using System;
using System.Linq;
using System.Text;

namespace MyLab.Logging
{
    /// <summary>
    /// Converts <see cref="LogEntity"/> to string
    /// </summary>
    public static class LogEntityFormatter
    {
        private static readonly string[] ExcludedConditions = new[]
        {
            AttributeNames.BaseExceptionMessage,
            AttributeNames.BaseExceptionStackTrace,
            AttributeNames.BaseExceptionType,
            AttributeNames.ExceptionMessage,
            AttributeNames.ExceptionStackTrace,
            AttributeNames.ExceptionType
        };

        public static Func<LogEntity, Exception, string> Func = (entity, exception) =>
        {
            var b = new StringBuilder();
            if (entity.Message != null)
                b.Append(entity.Message);
            if (b.Length != 0)
                b.AppendLine();
            b.AppendLine($"Id: {entity.InstanceId}");
            if (entity.Markers.Count != 0)
                b.AppendLine("Markers: " + string.Join(", ", entity.Markers));
            if (entity.Conditions.Count != 0)
                b.AppendLine("Conditions: " + string.Join(", ", entity.Conditions));
            foreach (var cc in entity.CustomConditions.Where(c => ExcludedConditions.All(ec => ec != c.Name)))
                b.AppendLine(cc.Name + ": " + cc.Value);

            return b.ToString();
        };
    }
}