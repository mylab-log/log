using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyLab.Log.Serializing.Json
{
    class LogContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            Predicate<object> shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => (shouldSerialize == null || shouldSerialize(obj)) && !IsEmptyCollection(property, obj);

            if (member is PropertyInfo)
            {
                //this is always `null`. Maybe except when property marked by Converter attribute 
                if (property.ItemConverter != null)
                    property.ItemConverter = new PropertyExceptionConverter(property.ItemConverter);
                property.ValueProvider = new PropertyExceptionWrapper(property.ValueProvider);
            }

            return property;
        }

        private bool IsEmptyCollection(JsonProperty property, object target)
        {
            object value;

            try
            {
                value = property.ValueProvider?.GetValue(target);
            }
            catch
            {
                return true;
            }

            if (value is PropertyExceptionDescriptor)
                return false;

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

        class PropertyExceptionWrapper : IValueProvider
        {
            private readonly IValueProvider _inner;

            public PropertyExceptionWrapper(IValueProvider inner)
            {
                _inner = inner;
            }

            public void SetValue(object target, object value)
            {
                _inner.SetValue(target, value);
            }

            public object GetValue(object target)
            {
                try
                {
                    return _inner.GetValue(target);
                }
                catch (Exception e)
                {
                    //For some reason, this turns into "null"
                    return new PropertyExceptionDescriptor(e);
                }
            }
        }

        class PropertyExceptionConverter : JsonConverter
        {
            private readonly JsonConverter _inner;

            public PropertyExceptionConverter(JsonConverter inner)
            {
                _inner = inner;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is PropertyExceptionDescriptor errorDescriptor)
                {
                    _inner.WriteJson(writer, errorDescriptor.ToString(), serializer);
                }
                else
                {
                    _inner.WriteJson(writer, value, serializer);
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return _inner.ReadJson(reader, objectType, existingValue, serializer);
            }

            public override bool CanConvert(Type objectType)
            {
                return _inner.CanConvert(objectType) || objectType == typeof(PropertyExceptionDescriptor);
            }
        }

        class PropertyExceptionDescriptor 
        {
            private readonly Exception _inner;

            public PropertyExceptionDescriptor(Exception inner)
            {
                _inner = inner;
            }

            public override string ToString()
            {
                return PropertyExceptionFormatter.ExceptionToString(_inner);
            }
        }
    }
}