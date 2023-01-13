using System;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class JTokenConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return typeof(JToken).IsAssignableFrom(type);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var str = ((JToken)value).ToString(Formatting.Indented).Replace("\r\n", "\n");
            emitter.Emit(new Scalar(str));
        }
    }
}