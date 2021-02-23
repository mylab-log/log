using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log.Serializing;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Log.Tests
{
    public class LogEntitySerializerBehavior
    {
        private readonly ITestOutputHelper _output;

        private readonly IReadOnlyDictionary<string, ILogEntitySerializer> _serializers = new Dictionary<string, ILogEntitySerializer>
        {
            {"yaml", new YamlLogEntitySerializer()},
            {"json", new JsonLogEntitySerializer()}
        };

        public LogEntitySerializerBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("yaml", "Time: 1990-02-03T10:23:44.123")]
        [InlineData("json", "\"Time\": \"1990-02-03T10:23:44.123\"")]
        public void ShouldSerializeDt(string serializer, string expected)
        {
            //Arrange
            var dt = new DateTime(1990, 02, 03, 10, 23, 44).AddMilliseconds(123);

            var log = new LogEntity
            {
                Time = dt
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Message: foo")]
        [InlineData("json", "\"Message\": \"foo\"")]
        public void ShouldSerializeContent(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Message = "foo"
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Labels:")]
        [InlineData("yaml", "foo: bar")]
        [InlineData("yaml", "baz: quux")]
        [InlineData("json", "\"Labels\": {")]
        [InlineData("json", "\"foo\": \"bar\",")]
        [InlineData("json", "\"baz\": \"quux\"")]
        public void ShouldSerializeLabels(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Labels =
                {
                    { "foo", "bar" },
                    { "baz", "quux" }
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Labels: {}")]
        [InlineData("json", "\"Labels\": {}")]
        public void ShouldNotSerializeEmptyLabels(string serializer, string notExpected)
        {
            //Arrange
            var log = new LogEntity();

            //Act & Assert
            DoesNotContainsActAndAssert(log, notExpected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Facts:")]
        [InlineData("yaml", "foo: bar")]
        [InlineData("yaml", "baz: quux")]
        [InlineData("json", "\"Facts\": {")]
        [InlineData("json", "\"foo\": \"bar\",")]
        [InlineData("json", "\"baz\": \"quux\"")]
        public void ShouldSerializeFacts(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    { "foo", "bar" },
                    { "baz", "quux" }
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Facts:")]
        [InlineData("yaml", "foo:")]
        [InlineData("yaml", "Type: MyLab.Log.Tests.LogEntitySerializerBehavior+FactValue")]
        [InlineData("yaml", "Id: 20")]
        [InlineData("yaml", "Values:")]
        [InlineData("yaml", "- bar")]
        [InlineData("yaml", "- baz")]
        [InlineData("json", "\"Facts\": {")]
        [InlineData("json", "\"foo\": {")]
        [InlineData("json", "\"Id\": 20,")]
        [InlineData("json", "\"Values\": [")]
        [InlineData("json", "\"bar\",")]
        [InlineData("json", "\"baz\"")]
        public void ShouldSerializeNestedObjects(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    {
                        "foo", 
                        new FactValue
                        {
                            Id = 20,
                            Values =
                            {
                                "bar",
                                "baz"
                            }
                        }
                    }
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "foo: bar-baz")]
        [InlineData("json", "\"foo\": \"bar-baz\"")]
        public void ShouldSerializeLogValueFact(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    { "foo", new ConcatLogStringVal("bar", "baz")}
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Facts: {}")]
        [InlineData("json", "\"Facts\": {}")]
        public void ShouldNotSerializeEmptyFacts(string serializer, string notExpected)
        {
            //Arrange
            var log = new LogEntity();

            //Act & Assert
            DoesNotContainsActAndAssert(log, notExpected, serializer);
        }

        void ContainsActAndAssert(LogEntity log, string expected, string serializerKey)
        {
            var actual = Serialize(serializerKey, log);
            AssertContains(actual, expected);
        }

        void DoesNotContainsActAndAssert(LogEntity log, string notExpected, string serializerKey)
        {
            var actual = Serialize(serializerKey, log);
            AssertDoesNotContain(actual, notExpected);
        }

        string Serialize(string serializerKey, LogEntity logEntity)
        {
            if (!_serializers.TryGetValue(serializerKey, out var serializer))
                throw new NotSupportedException($"Serializer '{serializerKey}' not supported");

            var serialized = serializer.Serialize(logEntity);

            _output.WriteLine(serialized);

            return serialized;
        }

        void AssertContains(string result, string expected)
        {
            var serializedStrings = result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            Assert.Contains(serializedStrings, s => s == expected);
        }

        void AssertDoesNotContain(string result, string notExpected)
        {
            var serializedStrings = result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            Assert.DoesNotContain(serializedStrings, s => s == notExpected);
        }

        class FactValue
        {
            public int Id { get; set; }

            public List<string> Values { get; } = new List<string>();
        }

        class ConcatLogStringVal : ILogStringValue
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
    }
}
