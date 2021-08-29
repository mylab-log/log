using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log;
using MyLab.Log.Serializing;
using MyLab.Log.Serializing.Json;
using MyLab.Log.Serializing.Yaml;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public partial class LogEntitySerializerBehavior
    {
        private readonly ITestOutputHelper _output;

        private readonly IReadOnlyDictionary<string, ILogEntitySerializer> _serializers =
            new Dictionary<string, ILogEntitySerializer>
            {
                {"yaml", new YamlLogEntitySerializer()},
                {"json", new JsonLogEntitySerializer()}
            };

        public LogEntitySerializerBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private void ContainsActAndAssert(LogEntity log, string expected, string serializerKey)
        {
            var actual = Serialize(serializerKey, log);
            AssertContains(actual, expected);
        }

        private void DoesNotContainsActAndAssert(LogEntity log, string notExpected, string serializerKey)
        {
            var actual = Serialize(serializerKey, log);
            AssertDoesNotContain(actual, notExpected);
        }

        private string Serialize(string serializerKey, LogEntity logEntity)
        {
            if (!_serializers.TryGetValue(serializerKey, out var serializer))
                throw new NotSupportedException($"Serializer '{serializerKey}' not supported");

            var serialized = serializer.Serialize(logEntity);

            _output.WriteLine(serialized);

            return serialized;
        }

        private void AssertContains(string result, string expected)
        {
            var serializedStrings = result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            Assert.Contains(serializedStrings, s => s == expected);
        }

        private void AssertDoesNotContain(string result, string notExpected)
        {
            var serializedStrings = result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            Assert.DoesNotContain(serializedStrings, s => s == notExpected);
        }

        private class FactValueWithArray
        {
            public int Id { get; set; }

            public string[] Values { get; set; }
        }

        private class FactValue
        {
            public int Id { get; set; }

            public List<string> Values { get; set; } = new List<string>();
        }

        private class ConcatLogStringVal : ILogStringValue
        {
            private readonly string _val1;
            private readonly string _val2;

            public ConcatLogStringVal(string val1, string val2)
            {
                _val1 = val1;
                _val2 = val2;
            }

            public string ToLogString()
            {
                return _val1 + "-" + _val2;
            }
        }

        class FactValueWithPropertyException
        {
            public int Value => throw new Exception("Property error");
        }
    }
}