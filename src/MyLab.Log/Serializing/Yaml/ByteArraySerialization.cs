using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace MyLab.Log.Serializing.Yaml
{
    class ByteArraySerialization
    {
        private readonly IEmitter _emitter;
        public string EmptyString { get; set; } = "[empty-bin]";
        public string TooLongStringPattern { get; set; } = "[binary >{0} bytes]";

        public long LengthLimit { get; set; } = 1024;

        public ByteArraySerialization(IEmitter emitter)
        {
            _emitter = emitter;
        }

        public void Write(long length, Func<byte[]> binProvider)
        {
            if (length == 0)
            {
                _emitter.Emit(new Scalar(EmptyString));
            }
            else
            {
                string strVal = length > LengthLimit
                    ? string.Format(TooLongStringPattern, LengthLimit)
                    : Convert.ToBase64String(binProvider());

                _emitter.Emit(new Scalar(strVal));
            }
        }
    }
}