using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.Log;

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

            WriteLogs(logger, LogEntityFormatter.Yaml);
            WriteLogs(logger, LogEntityFormatter.Json);

            WriteLogWithExceptionStuff(logger, LogEntityFormatter.Yaml);
        }

        private static void WriteLogWithExceptionStuff(ILogger<Program> logger, Func<LogEntity, Exception, string> formatter)
        {
            Exception exception;
            try
            {
                var ex = new InvalidOperationException("Inner message");
                
                var eData = new ExceptionLogData(ex);
                eData.AddFact("Inner exception fact", "inner fact");
                eData.AddLabel("error", "true");

                throw ex;
            }
            catch (Exception e)
            {
                exception = e;
            }

            var logEntity = new LogEntity
            {
                Message = "Error", 
                Exception = exception
            };


            logger.Log(LogLevel.Error, default, logEntity, null, formatter);
        }

        private static void WriteLogs(ILogger logger, Func<LogEntity, Exception, string> formatter)
        {
            Console.WriteLine("=============================================\nINFORMATION LOG:\n=============================================");
            LogInfo(logger, formatter);
            Thread.Sleep(100);
            Console.WriteLine("=============================================\nSEPARATED EXCEPTION ERROR LOG:\n=============================================");
            LogSeparateError(logger, formatter);
            Thread.Sleep(100);
            Console.WriteLine("=============================================\nINCLUDED EXCEPTION ERROR LOG:\n=============================================");
            LogIncludedError(logger, formatter);
            Thread.Sleep(100);
            Console.WriteLine("=============================================");
        }

        private static void LogSeparateError(ILogger logger, Func<LogEntity, Exception, string> formatter)
        {
            var exceptionForLogging = CreateException();
            var logEntity = CreateLogEntity();

            logger.Log(LogLevel.Error, default, logEntity, exceptionForLogging, formatter);
        }

        private static void LogIncludedError(ILogger logger, Func<LogEntity, Exception, string> formatter)
        {
            var exceptionForLogging = CreateException();
            var logEntity = CreateLogEntity();
            logEntity.Exception = exceptionForLogging;

            logger.Log(LogLevel.Error, default, logEntity, exceptionForLogging, formatter);
        }

        private static void LogInfo(ILogger logger, Func<LogEntity, Exception, string> formatter)
        {
            var logEntity = CreateLogEntity();

            logger.Log(LogLevel.Information, default, logEntity, null, formatter);
        }

        static LogEntity CreateLogEntity()
        {
            return new LogEntity
            {
                Message = "Test message",
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
                    inner = new InvalidOperationException("Inner message");
                    var innerEData = new ExceptionLogData(inner);
                    innerEData.AddFact("Inner exception fact", "inner fact");
                    throw inner;
                }
                catch (Exception e)
                {
                    inner = e;
                }

                var outer = new NotSupportedException("Big exception", inner);
                var outerEData = new ExceptionLogData(inner);
                outerEData.AddLabel("unsuppoted", "true");
                throw outer;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
