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
        public void ShouldWriteScope()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            //Act
            using (logger.BeginScope("bar"))
            {
                logger.LogInformation("baz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.Scopes + ":", logString);
            Assert.Contains("String: bar", logString);
        }

        [Fact]
        public void ShouldWriteComplexScope()
        {
            //Arrange
            var scope = new TestScope
            {
                { "foo", "bar" },
                { "baz", "qoz" },
            };
            
            var logBuilder = new StringBuilder();
            var loggerFactory = PrepareLogger(logBuilder);

            var logger = loggerFactory.CreateLogger("foo");

            //Act
            using (logger.BeginScope(scope))
            {
                logger.LogInformation("baz");
            }

            var logString = logBuilder.ToString();

            _output.WriteLine(logString);

            //Assert
            Assert.Contains(PredefinedFacts.Scopes + ":", logString);
            Assert.Contains("TestScope:", logString);
            Assert.Contains("foo: bar", logString);
            Assert.Contains("baz: qoz", logString);
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
    }
}
