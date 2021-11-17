namespace MyLab.Log.Loggers
{
    class MyLabConsoleLoggerProvider : MyLabLoggerProvider
    {
        public MyLabConsoleLoggerProvider() : base(new ConsoleLogOutputWriter())
        {
        }
    }
}
