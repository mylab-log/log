using System;
using System.Collections.Generic;
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
                Id = Guid.NewGuid(),
                EventId = 100,
                Markers = new List<string> { "marker1", "marker2"},
                Attributes = new List<LogEntityAttribute> {
                    new LogEntityAttribute
                    {
                        Name = "Attr1",
                        Value = "Val1"
                    },
                    new LogEntityAttribute
                    {
                        Name = "Attr2",
                        Value = "Val2"
                    }
                },
                Content = "Hellow world!"
            };

            var exception = new Exception("Error message");

            var str = LogEntityFormatter.Func(l, exception);
            _output.WriteLine("Serialized:");
            _output.WriteLine(str);
        }
    }
}
