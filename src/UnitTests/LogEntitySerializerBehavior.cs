﻿using System;
using System.Reflection;
using MyLab.Log;
using Xunit;

namespace UnitTests
{
    public partial class LogEntitySerializerBehavior
    {
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
    }
}