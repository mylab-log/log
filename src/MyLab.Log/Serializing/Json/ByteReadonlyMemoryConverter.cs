using System;
using Newtonsoft.Json;

namespace MyLab.Log.Serializing.Json
{
    class ByteReadonlyMemoryConverter : JsonConverter<ReadOnlyMemory<byte>>
    {
        public override void WriteJson(JsonWriter writer, ReadOnlyMemory<byte> value, JsonSerializer serializer)
        {
            var mem = (ReadOnlyMemory<byte>)value;

            if (mem.IsEmpty)
            {
                writer.WriteValue("[empty]");
            }
            else
            {
                string strVal = mem.Length > 1024
                    ? "[binary >1024 bytes]"
                    : Convert.ToBase64String(mem.ToArray());

                writer.WriteValue(strVal);
            }

            
        }

        public override ReadOnlyMemory<byte> ReadJson(JsonReader reader, Type objectType, ReadOnlyMemory<byte> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}