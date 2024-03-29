﻿using System;
using YamlDotNet.Core;
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
            var byteArrSerializer = new ByteArraySerialization(emitter);

            var mem = (ReadOnlyMemory<byte>)value;

            byteArrSerializer.Write(mem.Length, () => mem.ToArray());
        }
    }
}