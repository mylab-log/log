using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace MyLab.Log.Serializing.Yaml
{
    class NullStringsEventEmitter : ChainedEventEmitter
    {
        public NullStringsEventEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.Value == null)
            {
                emitter.Emit(new Scalar("[null]"));
            }
            else
            {
                base.Emit(eventInfo, emitter);
            }
        }
    }
}