using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{

    /// <summary>
    /// Serializes <see cref="LogEntity"/> into JSON format
    /// </summary>
    public class JsonLogEntitySerializer : ILogEntitySerializer
    {
        /// <inheritdoc />
        public string Serialize(LogEntity logEntity)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new LogContractResolver(),
                Converters =
                {
                    new LogStringValueConverter(),
                    new ReflectionConverter(),
                    new JTokenConverter(),
                    new ByteReadonlyMemoryConverter()
                },
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff"
            };

            serializer.Error += (sender, args) =>
            {
                args.ErrorContext.Handled = true;
            };

            var sb = new StringBuilder();

            using (var tetWriter = new StringWriter(sb) { NewLine = "\n" })
            using (var jsonWriter = new JsonTextWriter(tetWriter))
            {
                serializer.Serialize(jsonWriter, logEntity);
            }

            return sb.ToString();
        }
    }
}