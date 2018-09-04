using System;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Logging.Tests
{
    public class LogEntityFormatterBehavior
    {
        private readonly ITestOutputHelper _output;

        public LogEntityFormatterBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Demo()
        {
            var l = new LogEntity
            {
                InstanceId = Guid.NewGuid(),
                EventId = 100,
                Markers = { "marker1", "marker2"},
                Conditions = { "condition1", "condition2" },
                CustomConditions = { new LogEntityCustomCondition("Attr1", "Val1"), new LogEntityCustomCondition("Attr2", "Val2") },
                Message = "Hellow world!"
            };

            var exception = new Exception("Error message");

            var str = LogEntityFormatter.Func(l, exception);
            _output.WriteLine("Serialized:");
            _output.WriteLine(str);
        }
    }
}
