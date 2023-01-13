using System;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class ByteReadonlyMemoryConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(ReadOnlyMemory<byte>);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var mem = (ReadOnlyMemory<byte>)value;

            if (mem.IsEmpty)
            {
                emitter.Emit(new Scalar("[empty]"));
            }
            else
            {
                string strVal = mem.Length > 1024
                    ? "[binary >1024 bytes]"
                    : Convert.ToBase64String(mem.ToArray());

                emitter.Emit(new Scalar(strVal));
            }
        }
    }
}