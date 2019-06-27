using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MyLab.Logging
{
    /// <summary>
    /// Converts <see cref="LogEntity"/> to string
    /// </summary>
    public static class LogEntityFormatter
    {
        private static readonly string[] ExcludedAttributes =
        {
            AttributeNames.Exception
        };

        /// <summary>
        /// Formatter function
        /// </summary>
        public static Func<LogEntity, Exception, string> Func = (entity, exception) =>
        {
            var b = new StringBuilder();
            if (entity.Content != null)
                b.Append(entity.Content);
            if (b.Length != 0)
                b.AppendLine();
            b.AppendLine($"Log id: {entity.Id}");
            b.AppendLine($"Log time: {entity.Time}");
            
            var eventIdText = entity.EventId != 0 ? entity.EventId.ToString() : "[not defined]";
            b.AppendLine($"Event id: {eventIdText}");
            
            if (entity.Markers != null && entity.Markers.Count != 0)
                b.AppendLine("Markers: " + string.Join(", ", entity.Markers));

            if (entity.Attributes != null && entity.Attributes.Count != 0)
            {
                foreach (var cc in entity.Attributes.Where(c => ExcludedAttributes.All(ec => ec != c.Name)))
                    b.AppendLine(cc.Name + ": " + AttrValueToString(cc.Value));
            }

            return b.ToString();
        };

        static string AttrValueToString(object attrValue)
        {
            if (attrValue == null) return "[null]";
            
            if(attrValue is ILogAttributeStringValue strVal)
                return strVal.ToLogString();

            var vTp = attrValue.GetType();

            if (vTp.IsPrimitive ||
                vTp == typeof(string) ||
                vTp == typeof(DateTime) ||
                vTp == typeof(Guid))
            {
                return attrValue.ToString();
            }

            return Environment.NewLine + ToJson(attrValue);
        }

        private static string ToJson(object attrValue)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            };
            
            var json = JsonConvert.SerializeObject(attrValue, settings);

            return "\t" + json.Replace(Environment.NewLine, Environment.NewLine + "\t");
        }
    }
}