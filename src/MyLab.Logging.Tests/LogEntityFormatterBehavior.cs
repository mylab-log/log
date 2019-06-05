using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.Logging.Tests
{
    public class LogEntityFormatterBehavior
    {
        private readonly ITestOutputHelper _output;

        private DateTime _testLogTime;
        private string _testLogTimeStr;
        private string _nl = Environment.NewLine;
        
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
        }
        
        [Fact]
        public void ShouldSerializeAttributes()
        {
            //Arrange
            var l = new LogEntity
            {
                Content = "foo",
                Time = _testLogTime,
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
            };
            
            //Act
            var str = LogEntityFormatter.Func(l, null);
            
            //Assert
            Assert.Equal($"foo{_nl}" +
                         $"Log id: 00000000-0000-0000-0000-000000000000{_nl}" + 
                         $"Log time: {_testLogTimeStr}{_nl}" +
                         $"Event id: [not defined]{_nl}" +
                         $"Attr1: Val1{_nl}" +  
                         $"Attr2: Val2{_nl}",  
                str);
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
        }
    }
}
