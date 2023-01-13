using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyLab.Log.Serializing.Json
{
    class JTokenConverter : JsonConverter<JToken>
    {
        public override void WriteJson(JsonWriter writer, JToken value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Formatting.None));
        }

        public override JToken ReadJson(JsonReader reader, Type objectType, JToken existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
