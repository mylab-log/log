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
        private static readonly string[] ExcludedAttributes =
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
            if (entity.Content != null)
                b.Append(entity.Content);
            if (b.Length != 0)
                b.AppendLine();
            b.AppendLine($"Id: {entity.Id}");
            if (entity.Markers != null && entity.Markers.Count != 0)
                b.AppendLine("Markers: " + string.Join(", ", entity.Markers));
            if(entity.Attributes != null && entity.Attributes.Count != 0)
            foreach (var cc in entity.Attributes.Where(c => ExcludedAttributes.All(ec => ec != c.Name)))
                b.AppendLine(cc.Name + ": " + cc.Value);

            return b.ToString();
        };
    }
}