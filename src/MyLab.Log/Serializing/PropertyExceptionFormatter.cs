using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyLab.Log.Serializing
{
    static class PropertyExceptionFormatter
    {
        public static string ExceptionToString(Exception exception)
        {
            var targetException = (exception is TargetInvocationException
                ? exception.InnerException
                : exception
                ) ?? exception;

            return $"[({targetException.GetType().Name}) {targetException.Message}]";
        }
    }
}
