using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ExceptionLogDataBehavior
    {
        private readonly ITestOutputHelper _output;

        public ExceptionLogDataBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldProvideFacts()
        {
            //Arrange
            var e = new Exception();
            var eData = new ExceptionLogData(e);
            eData.AddFact("foo", "bar");

            //Act
            var facts = eData
                .GetFacts()
                .ToArray();

            //Assert
            Assert.Single(facts);
            Assert.Equal("foo", facts.First().Key);
            Assert.Equal("bar", facts.First().Value);
        }

        [Fact]
        public void ShouldProvideLabels()
        {
            //Arrange
            var e = new Exception();
            var eData = new ExceptionLogData(e);
            eData.AddLabel("foo", "bar");

            //Act
            var labels = eData
                .GetLabels()
                .ToArray();

            //Assert
            Assert.Single(labels);
            Assert.Equal("foo", labels.First().Key);
            Assert.Equal("bar", labels.First().Value);
        }

        [Fact]
        public void ShouldReplaceLabelWithTheSameNames()
        {
            //Arrange
            var e = new Exception();
            var eData = new ExceptionLogData(e);
            eData.AddLabel("foo", "bar");
            eData.AddLabel("foo", "baz");

            //Act
            var labels = eData
                .GetLabels()
                .ToArray();

            //Assert
            Assert.Single(labels);
            Assert.Equal("foo", labels.First().Key);
            Assert.Equal("baz", labels.First().Value);
        }

        [Fact]
        public void ShouldReplaceFactWithTheSameNames()
        {
            //Arrange
            var e = new Exception();
            var eData = new ExceptionLogData(e);
            eData.AddFact("foo", "bar");
            eData.AddFact("foo", "baz");

            //Act
            var facts = eData
                .GetFacts()
                .ToArray();

            //Assert
            Assert.Single(facts);
            Assert.Equal("foo", facts.First().Key);
            Assert.Equal("baz", facts.First().Value);
        }

        [Fact]
        public void ShouldCalcTrace()
        {
            //Arrange
            var ex1 = new Exception("Error text");

            //Act
            var dto1 = ExceptionDto.Create(ex1);

            _output.WriteLine("TRACE: " + dto1.ExceptionTrace);

            //Assert
            Assert.Equal(dto1.ExceptionTrace, "cf60b784c483dd053f56c29afb02eb33");
        }

        [Fact]
        public void ShouldCalcEqualTrace()
        {
            //Arrange
            var ex1 = new Exception("Error text");
            var ex2 = new Exception("Error text");

            //Act
            var dto1 = ExceptionDto.Create(ex1);
            var dto2 = ExceptionDto.Create(ex2);
            
            _output.WriteLine("TRACE: " + dto1.ExceptionTrace);

            //Assert
            Assert.Equal(dto1.ExceptionTrace, dto2.ExceptionTrace);
        }

        [Fact]
        public void ShouldCalcSameTraceWithSameStacktraceAndDiffLines()
        {
            //Arrange
            var ex1Init = new Exception("Error text");
            var ex2Init = new Exception("Error text");

            Exception ex1, ex2;

            try
            {
                throw ex1Init;
            }
            catch (Exception e)
            {
                ex1 = e;
            }

            try
            {
                throw ex2Init;
            }
            catch (Exception e)
            {
                ex2 = e;
            }

            //Act
            var dto1 = ExceptionDto.Create(ex1);
            var dto2 = ExceptionDto.Create(ex2);

            _output.WriteLine("StackTrace1: " + ex1.StackTrace);
            _output.WriteLine("StackTrace2: " + ex2.StackTrace);
            _output.WriteLine("TRACE: " + dto1.ExceptionTrace);

            //Assert
            Assert.Equal(dto1.ExceptionTrace, dto2.ExceptionTrace);
        }
    }
}
