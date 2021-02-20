using System;
using System.Linq;
using Xunit;

namespace MyLab.Logging.Tests
{
    public class ExceptionExtensionsBehavior
    {
        [Fact]
        public void ShouldProvideFacts()
        {
            //Arrange
            var e = new Exception();
            e.AndFactIs("foo", "bar");

            //Act
            var facts = e
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
            e.AndMarkAs("foo", "bar");

            //Act
            var labels = e
                .GetLabels()
                .ToArray();

            //Assert
            Assert.Single(labels);
            Assert.Equal("foo", labels.First().Key);
            Assert.Equal("bar", labels.First().Value);
        }
    }
}
