using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyLab.Logging.Serializing
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
                }
            });
        }

        class EmptyCollectionContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                Predicate<object> shouldSerialize = property.ShouldSerialize;
                property.ShouldSerialize = obj => (shouldSerialize == null || shouldSerialize(obj)) && !IsEmptyCollection(property, obj);
                return property;
            }

            private bool IsEmptyCollection(JsonProperty property, object target)
            {
                var value = property.ValueProvider?.GetValue(target);
                if (value is ICollection collection && collection.Count == 0)
                    return true;

                if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    return false;

                var countProp = property.PropertyType?.GetProperty("Count");
                if (countProp == null)
                    return false;

                var count = (int)countProp.GetValue(value, null);
                return count == 0;
            }
        }

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
}