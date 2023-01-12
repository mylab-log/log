using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Log.Scopes;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class MyLabFormatterBehavior
    {
        private readonly ITestOutputHelper _output;

        public MyLabFormatterBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldWriteCoreLogsAsLogEntity()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            //Act
            logger.LogInformation("bar");

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains("Message: bar", logString);
            Assert.Contains("Facts:", logString);
        }

        [Fact]
        public void ShouldWriteCategory()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            //Act
            logger.LogInformation("baz");

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.Category + ": foo", logString);
        }

        [Fact]
        public void ShouldWriteTraceId()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            var traceId = Guid.NewGuid().ToString("N");
            var hostingScope = new HostingLogScope(traceId);

            //Act
            using (logger.BeginScope(hostingScope))
            {
                logger.LogInformation("baz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.TraceId + ": " + traceId, logString);
        }

        [Fact]
        public void ShouldWriteScopeFacts()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            var scopeFacts = new Dictionary<string, object>
            {
                { "bar", "baz" }
            };
            var factScope = new FactLogScope(scopeFacts);

            //Act
            using (logger.BeginScope(factScope))
            {
                logger.LogInformation("qoz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains("bar: baz", logString);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldIncludeSetOption(bool includeScopes)
        {
            //Arrange
            var logBuilder = new StringBuilder();

            var logWriter = new StringWriter(logBuilder);

            var loggerFactory = LoggerFactory.Create(b => b
                .AddConsole(o => o.FormatterName = "mylab")
                .AddMyLabFormatter(logWriter, opt => opt.IncludeScopes = includeScopes));

            var logger = loggerFactory.CreateLogger("foo");

            var scope1 = new TestLogScopes1("foo", "bar");
            var scope2 = new TestLogScopes2("baz", "qoz");

            //Act
            using (logger.BeginScope(scope1))
            using (logger.BeginScope(scope2))
            {
                logger.LogInformation("qoz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert

            if (includeScopes)
            {
                Assert.Contains(PredefinedFacts.Scopes + ":", logString);
                Assert.Contains(nameof(TestLogScopes1) + ":", logString);
                Assert.Contains("foo: bar", logString);
                Assert.Contains(nameof(TestLogScopes2) + ":", logString);
                Assert.Contains("baz: qoz", logString);
            }
            else
            {
                Assert.DoesNotContain(PredefinedFacts.Scopes + ":", logString);
                Assert.DoesNotContain(nameof(TestLogScopes1) + ":", logString);
                Assert.DoesNotContain("foo: bar", logString);
                Assert.DoesNotContain(nameof(TestLogScopes2) + ":", logString);
                Assert.DoesNotContain("baz: qoz", logString);
            }
        }

        ILoggerFactory PrepareLogger(StringBuilder logStringBuilder)
        {
            var logWriter = new StringWriter(logStringBuilder);

            return LoggerFactory.Create(b => b
                .AddConsole(o => o.FormatterName = "mylab")
                .AddMyLabFormatter(logWriter));
        }

        class TestLogScopes1 : Dictionary<string, object>
        {
            public TestLogScopes1(string key, string value)
            {
                Add(key, value);
            }
        }

        class TestLogScopes2 : Dictionary<string, object>
        {
            public TestLogScopes2(string key, string value)
            {
                Add(key, value);
            }
        }

        class HostingLogScope : List<KeyValuePair<string, object>>
        {
            public HostingLogScope(string traceId)
            {
                Add(new KeyValuePair<string, object>("TraceId", traceId));
            }
        }

    }
}
