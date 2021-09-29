using System;
using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{
    class LogStringValueConverter : JsonConverter<ILogStringValue>
    {
        public override void WriteJson(JsonWriter writer, ILogStringValue value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToLogString());
        }

        public override ILogStringValue ReadJson(JsonReader reader, Type objectType, ILogStringValue existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}