using System;
using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace MyLab.Log.Serializing.Yaml
{
    class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        public YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor)
        {
        }

        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            if (IsEnumerable(value.Type) && IsEmptyCollection(value))
                return false;

            return base.Enter(value, context);
        }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (IsEnumerable(value.Type) && IsEmptyCollection(value))
                return false;

            return base.EnterMapping(key, value, context);
        }

        private bool IsEmptyCollection(IObjectDescriptor value)
        {
            if (value.Value == null)
                return true;

            if (value.Value is IEnumerable enumerable)
                return !enumerable.GetEnumerator().MoveNext();

            return false;
        }

        bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}