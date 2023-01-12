using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void ShouldAddLogScopeIntoServiceScope()
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

            
            using var scope1 = services.CreateScope();
            using var scope2 = services.CreateScope();

            var logger1 = scope1.ServiceProvider.GetRequiredService<ILogger<LogScopingResearchTests>>();
            
            var logScope1 = CreateFactLogScope("foo", "bar");
            
            var logEntity = new LogEntity
            {
                Message = "ololo"
            };

            IDisposable lScope1 = null;

            //Act

            var task1 = Task.Run(() =>
            {
                lScope1 = logger1.BeginScope(logScope1);
                var lWrtr1 = scope1.ServiceProvider.GetRequiredService<LogWriterService>();

                lWrtr1.Write(logEntity);
            });
            task1.Wait();

            var task2 = Task.Run(() =>
            {
                var lWrtr2 = scope2.ServiceProvider.GetRequiredService<LogWriterService>();

                lWrtr2.Write(logEntity);
                lScope1?.Dispose();
                lWrtr2.Write(logEntity);
            });
            task2.Wait();

            var logOutput = logBuilder.ToString();
            _output.WriteLine(logOutput);

            //Assert
            
            Assert.Empty(logEntity.Facts);
        }

        FactLogScope CreateFactLogScope(string key, string value)
        {
            var facts = new Dictionary<string, object>
            {
                { key, value }
            };
            return new FactLogScope(facts);
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
