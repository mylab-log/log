using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class ReflectionConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type.Namespace?.StartsWith("System.Reflection") ?? false;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit(new Scalar(value.ToString()));
        }
    }
}