using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class TypePropertyDescriptor : IPropertyDescriptor
    {
        private readonly string _typeName;

        public string Name { get; set; } = "Type";
        public bool CanWrite { get; set; } = false;
        public Type Type { get; set; } = typeof(string);
        public Type TypeOverride { get; set; }
        public int Order { get; set; }
        public ScalarStyle ScalarStyle { get; set; }

        public TypePropertyDescriptor(string typeName)
        {
            _typeName = typeName;
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return null;
        }

        public IObjectDescriptor Read(object target)
        {
            return new ObjectDescriptor(_typeName, typeof(string), typeof(string));
        }

        public void Write(object target, object value)
        {

        }
    }
}