using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace MyLab.Log.Serializing.Yaml
{
    class PropertyExceptionWrapper : TypeInspectorSkeleton
    {
        private readonly ITypeInspector _innerInspector;

        public PropertyExceptionWrapper(ITypeInspector innerInspector)
        {
            _innerInspector = innerInspector;
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            return _innerInspector
                .GetProperties(type, container)
                .Select(p => new ExceptionPropertyWrappedDescriptor(p));
        }

        class ExceptionPropertyWrappedDescriptor : IPropertyDescriptor
        {
            private readonly IPropertyDescriptor _inner;

            public string Name => _inner.Name;
            public bool CanWrite => _inner.CanWrite;
            public Type Type => _inner.Type;
            public Type TypeOverride
            {
                get => _inner.TypeOverride;
                set => _inner.TypeOverride = value;
            }
            public int Order
            {
                get => _inner.Order;
                set => _inner.Order = value;
            }
            public ScalarStyle ScalarStyle
            {
                get => _inner.ScalarStyle;
                set => _inner.ScalarStyle = value;
            }

            public ExceptionPropertyWrappedDescriptor(IPropertyDescriptor inner)
            {
                _inner = inner;
            }

            public T GetCustomAttribute<T>() where T : Attribute
            {
                return _inner.GetCustomAttribute<T>();
            }

            public IObjectDescriptor Read(object target)
            {
                try
                {
                    return _inner.Read(target);
                }
                catch (Exception e)
                {
                    var msg = PropertyExceptionFormatter.ExceptionToString(e);
                    return new ObjectDescriptor(msg, typeof(string), typeof(string));
                }
            }

            public void Write(object target, object value)
            {
                _inner.Write(target, value);
            }
        }
    }
}
