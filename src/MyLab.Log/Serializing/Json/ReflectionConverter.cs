using System;
using System.Reflection;
using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{
    class ReflectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString() ?? "[null]");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType?.Namespace?.StartsWith("System.Reflection") ?? false;
        }
    }
}