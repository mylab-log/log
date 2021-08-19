﻿using Newtonsoft.Json;

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
                ContractResolver = new EmptyCollectionContractResolver(),
                Converters =
                {
                    new LogStringValueConverter()
                },
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff"
            });
        }
    }
}