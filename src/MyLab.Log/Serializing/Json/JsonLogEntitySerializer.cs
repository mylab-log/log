using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{

    /// <summary>
    /// Serializes <see cref="LogEntity"/> into JSON format
    /// </summary>
    public class JsonLogEntitySerializer : ILogEntitySerializer
    {
        public string Serialize(LogEntity logEntity)
        {
            return JsonConvert.SerializeObject(logEntity, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new LogContractResolver(),
                Converters =
                {
                    new LogStringValueConverter(),
                    new ReflectionConverter(),
                },
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;

                },
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff"
            });
        }
    }
}