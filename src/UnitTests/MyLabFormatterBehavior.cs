using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using MyLab.Log;
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

            //Act
            using (logger.BeginScope(new HostingLogScope(null, traceId)))
            {
                logger.LogInformation("baz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.TraceId + ": " + traceId, logString);
        }

        [Fact]
        public void ShouldWriteRequestId()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            var requestId = Guid.NewGuid().ToString("N");

            //Act
            using (logger.BeginScope(new HostingLogScope(requestId, null)))
            {
                logger.LogInformation("baz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.RequestId + ": " + requestId, logString);
        }

        ILoggerFactory PrepareLogger(StringBuilder logStringBuilder)
        {
            var logWriter = new StringWriter(logStringBuilder);

            return LoggerFactory.Create(b => b
                .AddConsole(o => o.FormatterName = "mylab")
                .AddMyLabFormatter(logWriter));
        }

        class TestScope : Dictionary<string, object>
        {

        }

        class HostingLogScope : List<KeyValuePair<string, object>>
        {
            public HostingLogScope(string requestId, string traceId)
            {
                if(requestId != null)
                    Add(new KeyValuePair<string, object>("RequestId", requestId));

                if (traceId != null)
                    Add(new KeyValuePair<string, object>("TraceId", traceId));
            }
        }

    }
}
