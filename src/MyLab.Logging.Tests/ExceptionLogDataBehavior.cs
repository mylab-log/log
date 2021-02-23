using System;
using System.Linq;
using Xunit;

namespace MyLab.Logging.Tests
{
    public class ExceptionLogDataBehavior
    {
        [Fact]
        public void ShouldProvideFacts()
        {
            //Arrange
            var e = new Exception();
            var eData = new ExceptionLogData(e);
            eData.AddFact("foo", "bar");

            //Act
            var facts = eData
                .GetLabels()
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
    }
}
