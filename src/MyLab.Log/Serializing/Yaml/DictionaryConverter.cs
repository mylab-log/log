using System;
using System.Collections;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MyLab.Log.Serializing.Yaml
{
    class Dictionary : IYamlTypeConverter
    {

        public bool Accepts(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var dict = (IDictionary)value;

            emitter.Emit(new StreamStart());
            emitter.Emit(new MappingStart());

            var str = ((JToken)value).ToString(Formatting.Indented).Replace("\r\n", "\n");
            emitter.Emit(new Scalar(str));
        }
    }
}