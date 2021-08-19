using System;
using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{
    class LogStringValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ILogStringValue)value).ToLogString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ILogStringValue).IsAssignableFrom(objectType);
        }
    }
}