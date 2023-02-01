using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Log.Scopes;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class LogScopingResearchTests
    {
        private readonly ITestOutputHelper _output;

        public LogScopingResearchTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldAddFactsIntoLogScope()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var logWriter = new StringWriter(logBuilder);

            var services = new ServiceCollection()
                .AddLogging(lb => 
                    lb.AddConsole(o => o.FormatterName = "mylab")
                        .AddMyLabFormatter(logWriter)
                )
                .AddScoped<LogWriterService>()
                .BuildServiceProvider();

            
            var logger = services.GetRequiredService<ILogger<LogScopingResearchTests>>();
            
            var factLogScope = new FactLogScope("foo", "bar");
            
            var logEntity = new LogEntity
            {
                Message = "ololo"
            };

            //Act

            using (logger.BeginScope(factLogScope))
            {
                var lWrtr1 = services.GetRequiredService<LogWriterService>();

                lWrtr1.Write(logEntity);
            }

            var logOutput = logBuilder.ToString();
            _output.WriteLine(logOutput);

            var factsWordPos = logOutput.IndexOf("Facts:", StringComparison.Ordinal);
            var expectedFactOfPos = logOutput.IndexOf("foo: bar", StringComparison.Ordinal);

            //Assert

            Assert.NotEqual(-1, factsWordPos);
            Assert.NotEqual(-1, expectedFactOfPos);
            Assert.True(expectedFactOfPos > factsWordPos);
            Assert.Empty(logEntity.Facts);
        }

        [Fact]
        public void ShouldAddLabelsIntoLogScope()
        {
            //Arrange
            var logBuilder = new StringBuilder();
            var logWriter = new StringWriter(logBuilder);

            var services = new ServiceCollection()
                .AddLogging(lb =>
                    lb.AddConsole(o => o.FormatterName = "mylab")
                        .AddMyLabFormatter(logWriter)
                )
                .AddScoped<LogWriterService>()
                .BuildServiceProvider();


            var logger = services.GetRequiredService<ILogger<LogScopingResearchTests>>();

            var labelLogScope = new LabelLogScope("foo", "bar");

            var logEntity = new LogEntity
            {
                Message = "ololo"
            };

            //Act

            using (logger.BeginScope(labelLogScope))
            {
                var lWrtr1 = services.GetRequiredService<LogWriterService>();

                lWrtr1.Write(logEntity);
            }

            var logOutput = logBuilder.ToString();
            _output.WriteLine(logOutput);

            var labelsWordPos = logOutput.IndexOf("Labels:", StringComparison.Ordinal);
            var expectedFactOfPos = logOutput.IndexOf("foo: bar", StringComparison.Ordinal);

            //Assert

            Assert.NotEqual(-1, labelsWordPos);
            Assert.NotEqual(-1, expectedFactOfPos);
            Assert.True(expectedFactOfPos > labelsWordPos);
            Assert.Empty(logEntity.Facts);
        }

        class LogWriterService
        {
            private readonly ILogger _logger;

            public LogWriterService(ILogger<LogWriterService> logger)
            {
                _logger = logger;
            }

            public void Write(LogEntity logEntity)
            {
                _logger.Log(LogLevel.Information, default, logEntity, null, LogEntityFormatter.Yaml);
            }
        }
    }
}
