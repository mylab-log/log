using System;

namespace MyLab.Logging
{
    public static class LogEntityFormatter
    {
        public static Func<LogEntity, Exception, string> Func = (entity, exception) => 
            exception != null 
                ? exception.Message 
                : entity.Message;
    }
}