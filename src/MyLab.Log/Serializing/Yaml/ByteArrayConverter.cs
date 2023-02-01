using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class ByteArrayConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return typeof(byte[]) == type;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var byteArrSerializer = new ByteArraySerialization(emitter);

            var bin = (byte[])value;

            byteArrSerializer.Write(bin.Length, () => bin);
        }
    }
}