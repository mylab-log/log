using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.Logging;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(c => c.AddConsole().AddDebug());

            var sp = services.BuildServiceProvider();

            var logger = sp.GetService<ILogger<Program>>();

            Console.WriteLine("INFORMATION LOG:\n=============================================");
            LogInfo(logger);
            Thread.Sleep(100);
            Console.WriteLine("=============================================\nSEPARATED EXCEPTION ERROR LOG:\n=============================================");
            LogSeparateError(logger);
            Thread.Sleep(100);
            Console.WriteLine("=============================================\nINCLUDED EXCEPTION ERROR LOG:\n=============================================");
            LogIncludedError(logger);
            Thread.Sleep(100);
            Console.WriteLine("=============================================");
        }

        private static void LogSeparateError(ILogger<Program> logger)
        {
            var exceptionForLogging = CreateException();
            var logEntity = CreateLogEntity();

            logger.Log(LogLevel.Error, default, logEntity, exceptionForLogging, LogEntityFormatter.Func);
        }

        private static void LogIncludedError(ILogger<Program> logger)
        {
            var exceptionForLogging = CreateException();
            var logEntity = CreateLogEntity();
            logEntity.SetException(exceptionForLogging);

            logger.Log(LogLevel.Error, default, logEntity, exceptionForLogging, LogEntityFormatter.Func);
        }

        private static void LogInfo(ILogger<Program> logger)
        {
            var logEntity = CreateLogEntity();

            logger.Log(LogLevel.Information, default, logEntity, null, LogEntityFormatter.Func);
        }

        static LogEntity CreateLogEntity()
        {
            return new LogEntity
            {
                Content = "Test message",
                Labels =
                {
                    {"label1", "value1"},
                    {"label2", "value2"}
                },
                Facts =
                {
                    {"fact1", "fact1"},
                    {"fact2", "fact2"}
                }
            };
        }

        static Exception CreateException()
        {
            try
            {
                Exception inner;
                try
                {
                    throw new InvalidOperationException("Inner message");
                }
                catch (Exception e)
                {
                    inner = e;
                }
                throw new NotSupportedException("Big exception", inner);
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
