using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
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