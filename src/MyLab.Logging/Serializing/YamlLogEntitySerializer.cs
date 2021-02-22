using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;

namespace MyLab.Logging.Serializing
{
    /// <summary>
    /// Serializes <see cref="LogEntity"/> into YAML format
    /// </summary>
    public class YamlLogEntitySerializer : ILogEntitySerializer
    {
        private readonly ISerializer _serializer;

        public YamlLogEntitySerializer()
        {
            _serializer = new SerializerBuilder()
                .WithTypeConverter(new LogStringValueConverter())
                .WithTypeConverter(new DateTimeValueConverter())
                .WithTypeInspector(inner => new TypeNameInjectorTypeInspector(inner))
                .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();
        }

        public string Serialize(LogEntity logEntity)
        {
            return _serializer.Serialize(logEntity);
        }

        class TypeNameInjectorTypeInspector : TypeInspectorSkeleton
        {
            private readonly ITypeInspector _innerTypeDescriptor;

            public TypeNameInjectorTypeInspector(ITypeInspector innerTypeDescriptor)
            {
                this._innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException(nameof(innerTypeDescriptor));
            }

            public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
            {
                var props = new List<IPropertyDescriptor>(
                    _innerTypeDescriptor.GetProperties(type, container));

                if (!type.IsPrimitive &&
                   !type.IsValueType &&
                   type != typeof(string) &&
                   type != typeof(LogEntity) &&
                   type != typeof(ExceptionDto))
                {
                    props.Insert(0, new TypePropertyDescriptor(type.FullName));
                }
                return props;
            }
        }

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

        class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
        {
            public YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor)
            {
            }

            private bool IsEmptyCollection(IObjectDescriptor value)
            {
                if (value.Value == null)
                    return true;

                if (value.Value is IEnumerable enumerable)
                    return !enumerable.GetEnumerator().MoveNext();

                return false;
            }

            public override bool Enter(IObjectDescriptor value, IEmitter context)
            {
                if (IsEmptyCollection(value))
                    return false;

                return base.Enter(value, context);
            }

            public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
            {
                if (IsEmptyCollection(value))
                    return false;

                return base.EnterMapping(key, value, context);
            }
        }

        class LogStringValueConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return typeof(ILogStringValue).IsAssignableFrom(type);
            }

            public object ReadYaml(IParser parser, Type type)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                emitter.Emit(new Scalar(((ILogStringValue)value).ToLogString()));
            }
        }

        class DateTimeValueConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(DateTime);
            }

            public object ReadYaml(IParser parser, Type type)
            {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                emitter.Emit(new Scalar(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fff")));
            }
        }
    }
}
