using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Logging.Tests
{
    public class LogEntityFormatterBehavior
    {
        private readonly ITestOutputHelper _output;

        private readonly DateTime _testLogTime;
        private readonly string _testLogTimeStr;
        private static readonly string _nl = Environment.NewLine;
        
        public LogEntityFormatterBehavior(ITestOutputHelper output)
        {
            _output = output;

            _testLogTime = DateTime.Now;
            _testLogTimeStr = _testLogTime.ToString();
        }

        [Fact]
        public void ShouldSerializeSimpleMsg()
        {
            //Arrange
            var logId = Guid.NewGuid();
            var l = new LogEntity
            {
                Id = logId,
                Content = "foo",
                Time = _testLogTime
            };
            
            //Act
            var str = LogEntityFormatter.Func(l, null);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: {logId:D}{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: [not defined]{_nl}", 
                str);
            
            _output.WriteLine(str);
        }
        
        [Fact]
        public void ShouldSerializeEventId()
        {
            //Arrange
            var l = new LogEntity
            {
                Content = "foo",
                Time = _testLogTime,
                EventId = 1000
            };
            
            //Act
            var str = LogEntityFormatter.Func(l, null);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: 00000000-0000-0000-0000-000000000000{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: 1000{_nl}", 
                str);
            
            _output.WriteLine(str);
        }
        
        [Fact]
        public void ShouldSerializeMarkers()
        {
            //Arrange
            var l = new LogEntity
            {
                Content = "foo",
                Time = _testLogTime,
                Markers = new List<string> { "marker1", "marker2"}
            };
            
            //Act
            var str = LogEntityFormatter.Func(l, null);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: 00000000-0000-0000-0000-000000000000{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: [not defined]{_nl}" +
                         $"Markers: marker1, marker2{_nl}",  
                str);
            
            _output.WriteLine(str);
        }
        
        [Theory]
        [MemberData(nameof(GetAttrValues))]
        public void ShouldSerializeAttribute(string title, object value, string strValue)
        {
            //Arrange
            var l = new LogEntity
            {
                Content = "foo",
                Time = _testLogTime,
                Attributes = new List<LogEntityAttribute> {
                    new LogEntityAttribute
                    {
                        Name = "Attr",
                        Value = value
                    }
                },
            };
            
            //Act
            var str = LogEntityFormatter.Func(l, null);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: 00000000-0000-0000-0000-000000000000{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: [not defined]{_nl}" +  
                         $"Attr: {strValue}{_nl}",  
                str);
            
            _output.WriteLine(str);
        }

        [Fact]
        public void ShouldNotSerializeException()
        {
            //Arrange
            var l = new LogEntity
            {
                Content = "foo",
                Time = _testLogTime
            };
            
            var exception = new Exception("Error message");
            
            //Act
            var str = LogEntityFormatter.Func(l, exception);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: 00000000-0000-0000-0000-000000000000{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: [not defined]{_nl}", 
                str);
            
            _output.WriteLine(str);
        }
        
        public static IEnumerable<object[]> GetAttrValues()
        {
            var now = new DateTime(2001, 1, 1, 1, 1, 1);
            var guid = new Guid("7d2a9131-e4c4-4b46-ba70-6c87c93ffc1d");
            
            //Simple values
            yield return new object[] {"string", "foo", "foo"};
            yield return new object[] {"int", 1, "1"};
            yield return new object[] {"bool", true, "True"};
            yield return new object[] {"DateTime", now, now.ToString()};
            yield return new object[] {"Guid", guid, guid.ToString("D")};

            //Log string val
            yield return new Object []
            {
                "Log attribute string-object value", 
                new LogStringVal("foo", "bar"), 
                "foo-bar"
            };
            
            //Regular object value
            yield return new Object []
            {
                "Log attribute regular object value", 
                new RegularLogValue
                {
                    Prop1 = "foo",
                    Prop2 = 123
                }, 
                $"{_nl}\t{{{_nl}\t  \"Prop1\": \"foo\",{_nl}\t  \"Prop2\": 123{_nl}\t}}"
            };
        }
        
        class LogStringVal : ILogAttributeStringValue
        {
            private readonly string _val1;
            private readonly string _val2;

            public LogStringVal(string val1, string val2)
            {
                _val1 = val1;
                _val2 = val2;
            }
            
            public string ToLogString()
            {
                return _val1 + "-"+_val2;
            }
        }

        class RegularLogValue
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
        }   
    }
}
