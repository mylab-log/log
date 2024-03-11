using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MyLab.Log;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UnitTests
{
    public partial class LogEntitySerializerBehavior
    {
        [Theory]
        [InlineData("yaml", "Uri: http://foo.net/bar")]
        [InlineData("json", "\"Uri\": \"http://foo.net/bar\"")]
        public void ShouldSerializeAbsoluteUri(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    { "Uri", new Uri("http://foo.net/bar") }
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
        }

        [Theory]
        [InlineData("yaml", "Uri: /bar")]
        [InlineData("json", "\"Uri\": \"/bar\"")]
        public void ShouldSerializeRelativeUri(string serializer, string expected)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    { "Uri", new Uri("/bar", UriKind.Relative) }
                }
            };

            //Act & Assert
            ContainsActAndAssert(log, expected, serializer);
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
        [InlineData("json", "\"Message\": \"foo\",")]
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
        //[InlineData("yaml", "Type: MyLab.Log.Tests.LogEntitySerializerBehavior+FactValue")]
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

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldNotFailIfEmptyCollection(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    {"foo", new FactValueWithArray
                    {
                        Id = 123,
                        Values = null
                    }}
                }
            };

            //Act & Assert
            Serialize(serializer, log);
        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldNotFailIfNullCollection(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts =
                {
                    {"foo", new FactValueWithArray
                    {
                        Id = 123,
                        Values = new string[0]
                    }}
                }
            };

            //Act & Assert
            Serialize(serializer, log);
        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldSerializeNullFact(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts = { { "foo",  null} }
            };

            //Act & Assert
            Serialize(serializer, log);

        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldSerializeEmptyStringFact(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts = { { "foo", "" } }
            };

            //Act & Assert
            Serialize(serializer, log);

        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldSerializeWithPropertyException(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts = { { "foo", new FactValueWithPropertyException() }}
            };

            //Act & Assert
            Serialize(serializer, log);
        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldSerializeReflectionShortly(string serializer)
        {
            //Arrange
            var log = new LogEntity
            {
                Facts = { { "foo", MethodBase.GetCurrentMethod() } }
            };

            //Act 
            var str = Serialize(serializer, log);

            //Assert
            Assert.True(str.Split('\n').Length < 100);
        }

        [Theory]
        [MemberData(nameof(GetByteReadonlyTestCases))]
        public void ShouldSerializeReadOnlyMemory(string serializer, string content, string expected)
        {
            //Arrange
            var mem = content != null
                ? new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(content))
                : new ReadOnlyMemory<byte>();

            var log = new LogEntity
            {
                Facts = { { "foo", mem } }
            };

            //Act 
            var actual = Serialize(serializer, log);

            //Assert
            Assert.Contains(expected, actual);
        }

        [Theory]
        [InlineData("yaml")] 
        [InlineData("json")]
        public void ShouldSerializeEmptyDictionaryFactWithoutError(string serializer)
        {
            //Arrange
            var dict = new Dictionary<string, object>();
            
            var log = new LogEntity
            {
                Facts = { { "foo", dict } }
            };

            //Act & Assert
            Serialize(serializer, log);
        }

        [Theory]
        [InlineData("yaml")]
        [InlineData("json")]
        public void ShouldSerializeBytes(string serializer)
        {
            //Arrange
            var bin = Encoding.UTF8.GetBytes("ololo");

            var log = new LogEntity
            {
                Facts = { { "foo-bar", bin} }
            };

            //Act & Assert
            Serialize(serializer, log);
        }

        [Theory]
        [InlineData("yaml", "foo: >-\r\n    {\r\n      \"Id\": 123,\r\n      \"Values\": null\r\n    }")]
        [InlineData("json", "\"foo\": \"{\\\"Id\\\":123,\\\"Values\\\":null}\"")]
        public void ShouldSerializeJObject(string serializer, string expected)
        {
            //Arrange
            var fact = new FactValueWithArray
            {
                Id = 123,
                Values = null
            };

            var log = new LogEntity
            {
                Facts =
                {
                    { "foo", JObject.FromObject(fact) }
                }
            };

            //Act

            var actual = Serialize(serializer, log);

            //Assert
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void ShouldYamlSerializeExceptionDto()
        {
            //Arrange
            LogEntity logEntity = new LogEntity
            {
                Time = DateTime.MinValue,
                Message = "Test!"
            };

            ExceptionDto innerException;

            try
            {
                throw new Exception("Inner!");
            }
            catch (Exception e)
            {
                innerException = e;
            }

            try
            {
                throw new Exception("Error!");
            }
            catch (Exception e)
            {
                logEntity.Exception = ExceptionDto.Create(e);
            }

            logEntity.Exception.Aggregated = new[] { innerException };
            logEntity.Exception.Inner = innerException;
            logEntity.Facts.Add("foo", "bar");
            logEntity.Labels.Add("foo", "bar");

            //Act
            var actual = Serialize("yaml", logEntity).Trim();

            //Assert
            Assert.Contains(PredefinedLabels.ExceptionTrace+":", actual);
            Assert.Contains("ExceptionTrace:", actual);
            Assert.Contains("Type: System.Exception", actual);
            Assert.Contains("StackTrace:", actual);
            Assert.Contains("Inner:", actual);
            Assert.Contains("Aggregated:", actual);
        }

        public static IEnumerable<object[]> GetByteReadonlyTestCases()
        {
            return new[]
            {
                new object[]
                {
                    "json", null, "\"foo\": \"[empty-bin]\""
                },
                new object[]
                {
                    "yaml", null, "foo: '[empty-bin]'"
                },
                new object[]
                {
                    "json", "example", "\"foo\": \"ZXhhbXBsZQ==\""
                },
                new object[]
                {
                    "yaml", "example", "foo: ZXhhbXBsZQ=="
                },
                new object[]
                {
                    "json", string.Join(",", Enumerable.Repeat("example", 500)), "\"foo\": \"[binary >1024 bytes]\""
                },
                new object[]
                {
                    "yaml", string.Join(",", Enumerable.Repeat("example", 500)), "foo: '[binary >1024 bytes]'"
                },
            };
        }
    }
}
