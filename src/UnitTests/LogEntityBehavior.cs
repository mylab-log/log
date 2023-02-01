using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.Log;
using Xunit;

namespace UnitTests
{
    public class LogEntityBehavior
    {
        [Fact]
        public void ShouldAddExceptionTraceAsLabel()
        {
            //Arrange
            var exception = new Exception("ololo");
            var exceptionDto = ExceptionDto.Create(exception);

            var logEntity = new LogEntity();

            //Act
            logEntity.Exception = exceptionDto;

            var exceptionTraceLabel = logEntity.Labels.FirstOrDefault(l => l.Key == PredefinedLabels.ExceptionTrace);

            //Assert
            Assert.NotEqual(default, exceptionTraceLabel);
            Assert.Equal(exceptionDto.ExceptionTrace, exceptionTraceLabel.Value);
        }

        [Fact]
        public void ShouldUpdateExceptionTraceAsLabel()
        {
            //Arrange
            var exception1 = new Exception("ololo");
            var exception2 = new Exception("trololo");
            var exceptionDto1 = ExceptionDto.Create(exception1);
            var exceptionDto2 = ExceptionDto.Create(exception2);

            var logEntity = new LogEntity();

            //Act
            logEntity.Exception = exceptionDto1;
            logEntity.Exception = exceptionDto2;

            var exceptionTraceLabel = logEntity.Labels.FirstOrDefault(l => l.Key == PredefinedLabels.ExceptionTrace);

            //Assert
            Assert.NotEqual(default, exceptionTraceLabel);
            Assert.Equal(exceptionDto2.ExceptionTrace, exceptionTraceLabel.Value);
        }

        [Fact]
        public void ShouldRemoveExceptionTraceAsLabel()
        {
            //Arrange
            var exception = new Exception("ololo");
            var exceptionDto = ExceptionDto.Create(exception);

            var logEntity = new LogEntity();

            //Act
            logEntity.Exception = exceptionDto;
            logEntity.Exception = null;

            var exceptionTraceLabel = logEntity.Labels.FirstOrDefault(l => l.Key == PredefinedLabels.ExceptionTrace);

            //Assert
            Assert.Equal(default, exceptionTraceLabel);
        }
    }
}
