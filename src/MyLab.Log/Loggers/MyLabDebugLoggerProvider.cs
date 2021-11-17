namespace MyLab.Log.Loggers
{
    class MyLabDebugLoggerProvider : MyLabLoggerProvider
    {
        public MyLabDebugLoggerProvider() : base(new DebugLogOutputWriter())
        {
        }
    }
}