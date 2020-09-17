using System;
using System.Linq;
using Xunit;

namespace MyLab.Logging.Tests
{
    public class ExceptionExtensionsBehavior
    {
        [Fact]
        public void ShouldProvideConditions()
        {
            //Arrange
            var e = new Exception();
            e.AndFactIs("foo", "bar");

            //Act
            var conditions = e
                .GetConditions()
                .ToArray();

            //Assert
            Assert.Single(conditions);
            Assert.Equal("foo", conditions.First().Key);
            Assert.Equal("bar", conditions.First().Value);
        }
    }
}
