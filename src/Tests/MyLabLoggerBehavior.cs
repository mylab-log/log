using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Log.Loggers;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class MyLabLoggerBehavior
    {
        private readonly ITestOutputHelper _output;

        public MyLabLoggerBehavior(ITestOutputHelper output)
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
            var logWriter = new StringOutputWriter(logStringBuilder);
            var loggerProvider = new MyLabLoggerProvider(logWriter);

            var sp = new ServiceCollection()
                .AddLogging(l => l.AddProvider(loggerProvider))
                .BuildServiceProvider();

            return sp.GetService<ILoggerFactory>();
        }

        class TestScope : Dictionary<string, object>
        {

        }
    }
}
